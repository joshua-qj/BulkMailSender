using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.CommonService;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class SendEmailsUseCase : ISendEmailsUseCase {
        private readonly IMapper _mapper;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISaveEmailUseCase _saveEmailUseCase;
        private readonly IUpdateEmailStatusUseCase _updateEmailStatusUseCase;
        private readonly ISignalRNotificationService _notificationService;
        public SendEmailsUseCase(
            IMapper mapper,
            IEmailRepository emailRepository,
            IEmailSenderService emailSenderService,
            ISaveEmailUseCase saveEmailUseCase,
            IUpdateEmailStatusUseCase updateEmailStatusUseCase,
            IServiceProvider serviceProvider,
            ISignalRNotificationService notificationService) {
            _mapper = mapper;
            _emailRepository = emailRepository;
            _emailSenderService = emailSenderService;
            _saveEmailUseCase = saveEmailUseCase;
            _updateEmailStatusUseCase = updateEmailStatusUseCase;
            _notificationService = notificationService;
        }


        public async Task ExecuteAsync(IEnumerable<EmailDto> emails) {
            SemaphoreSlim semaphore = new SemaphoreSlim(1,4); // Limit concurrency to 4 tasks
            var tasks = new List<Task>();
            var failedEmails = new ConcurrentBag<string>(); // Use thread-safe collection for concurrency
            
            foreach (var emailDto in emails) {
                // Create tasks without blocking the loop
                tasks.Add(Task.Run(async () => {
                    Console.WriteLine($"Task ID: {Task.CurrentId}");
                    await semaphore.WaitAsync(); // Limit concurrency
                    string errorMessage = string.Empty;

                    try {
                        // Map EmailDto to Email entity
                        Domain.Entities.Email.Email email = _mapper.Map<Domain.Entities.Email.Email>(emailDto);

                        // Validate and set requester
                        try {
                            var requester = await _emailRepository.GetRequesterByIdAsync(emailDto.RequesterID);
    
                            if (requester is null) {
                                errorMessage = "Cannot find requester.";
                                failedEmails.Add(emailDto.EmailTo);
                                return;
                            }
                            email.SetRequester(requester);
                        }
                        catch (Exception requesterEx) {
                            errorMessage = requesterEx.Message;
                            failedEmails.Add(emailDto.EmailTo);
                            return; // Skip this email
                        }

                        // Save email and update ID
                        var emailDtoSave = _mapper.Map<EmailDto>(email);
                     //   var saveEmailUseCase1 = _serviceProvider.GetRequiredService<ISaveEmailUseCase>();
                        Console.WriteLine($"saveEmailUseCase from sendemailsusecase  . HashCode: {_saveEmailUseCase.GetHashCode()}");
                        Console.WriteLine($"_emailRepository  from sendemailsusecase . HashCode: {_emailRepository.GetHashCode().ToString()}");
                        Console.WriteLine($"Thread  from sendemailsusecase : {Thread.CurrentThread.ManagedThreadId}");
                        var savedEmailDtoResult = await _saveEmailUseCase.ExecuteAsync(emailDtoSave);
                        email.Id = savedEmailDtoResult.Id;

                        if (!email.IsValid()) {
                            errorMessage = "Invalid email data.";
                            failedEmails.Add(emailDto.EmailTo);
                            return;
                        }

                        // Send email
                        try {
                            var result = await _emailSenderService.SendAsync(email);
                            email.UpdateDeliveryStatus(result.IsSuccess ? null : result.ErrorMessage);

                            // Update email status
                            emailDtoSave = _mapper.Map<EmailDto>(email);
                            await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, result.ErrorMessage);
                        }
                        catch (Exception sendEx) {
                            failedEmails.Add(emailDto.EmailTo);
                            errorMessage = sendEx.Message;
                        }
                    }
                    catch (Exception ex) {
                        failedEmails.Add(emailDto.EmailTo);
                        errorMessage = ex.Message;
                    }
                    finally {
                        // Notify email status
                        if (emailDto.BatchID.HasValue) {
                            try {
                                var finalStatus = new EmailStatusUpdateEventDto {
                                    JobId = (Guid)emailDto.BatchID,
                                    Status = "Completed",
                                    Message = errorMessage,
                                    Sent = 1,
                                    Total = emails.Count(),
                                    FailedEmails = failedEmails.ToList(),
                                    EmailTo = emailDto.EmailTo,
                                    UpdatedAt = DateTime.UtcNow
                                };
                             await _notificationService.NotifyEmailStatusAsync(emailDto.BatchID.Value, finalStatus);
                            }
                            catch (Exception ex) {
                                Console.WriteLine($"Failed to notify email status: {ex.Message}");
                            }
                        }

                        semaphore.Release(); // Release semaphore
                    }
                }));
            }

            // Wait for all tasks to complete
            await Task.WhenAll(tasks);
        }
    }
}