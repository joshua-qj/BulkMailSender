using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.CommonService;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using MailKit.Security;
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


        public async Task ExecuteAsync(IEnumerable<EmailDto> emails, DateTime? startTime, int? emailCount, RequesterDto? requesterDto, ConcurrentDictionary<Guid, EmailFailureRecord>? failedEmails = null) {
            if (emails == null || !emails.Any()) return;
            SemaphoreSlim semaphore = new SemaphoreSlim(1, 8); // Limit concurrency to 4 tasks
            var tasks = new List<Task>();
            failedEmails ??= new ConcurrentDictionary<Guid, EmailFailureRecord>();
            // Create SmtpClient only if RequesterDto is provided
            MailKit.Net.Smtp.SmtpClient? smtpClient = null;
            //using MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient();
            if (requesterDto != null) {
                smtpClient = new MailKit.Net.Smtp.SmtpClient();
                await smtpClient.ConnectAsync(requesterDto.Server.ServerName, requesterDto.Server.Port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(requesterDto.LoginName, requesterDto.Password);

            }
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
                                failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                                    Email = emailDto.EmailTo,
                                    ErrorMessage = errorMessage
                                });
                                return;
                            }
                            email.SetRequester(requester);
                        }
                        catch (Exception requesterEx) {
                            errorMessage = requesterEx.Message;
                            failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                                Email = emailDto.EmailTo,
                                ErrorMessage = errorMessage
                            });
                            return; // Skip this email
                        }

                        // Save email and update ID
                        var emailDtoSave = _mapper.Map<EmailDto>(email);
                        //   var saveEmailUseCase1 = _serviceProvider.GetRequiredService<ISaveEmailUseCase>();
                        var savedEmailDtoResult = await _saveEmailUseCase.ExecuteAsync(emailDtoSave);
                        email.Id = savedEmailDtoResult.Id;

                        if (!email.IsValid()) {
                            errorMessage = "Invalid email data.";
                            // failedEmails.Add(new KeyValuePair<string, string>(emailDto.EmailTo, errorMessage));
                            failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                                Email = emailDto.EmailTo,
                                ErrorMessage = errorMessage
                            });
                            return;
                        }

                        // Send email
                        try {
                            var result = await _emailSenderService.SendAsync(email, smtpClient);
                            email.UpdateDeliveryStatus(result.IsSuccess ? null : result.ErrorMessage);

                            // Update email status
                            emailDtoSave = _mapper.Map<EmailDto>(email);
                            await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, result.ErrorMessage);
                        }
                        catch (Exception sendEx) {
                            // failedEmails.Add(new KeyValuePair<string, string>(emailDto.EmailTo, sendEx.Message));
                            //  failedEmails.TryAdd(emailDto.EmailTo, sendEx.Message);
                            errorMessage = sendEx.Message;
                            failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                                Email = emailDto.EmailTo,
                                ErrorMessage = errorMessage
                            });
                            return;
                        }
                    }
                    catch (Exception ex) {
                        // failedEmails.Add(new KeyValuePair<string, string>(emailDto.EmailTo, ex.Message));
                        //failedEmails.TryAdd(emailDto.EmailTo, ex.Message);
                        errorMessage = ex.Message;
                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                            Email = emailDto.EmailTo,
                            ErrorMessage = errorMessage
                        });
                        return;
                    }
                    finally {
                        // Notify email status
                        if (emailDto.BatchID.HasValue) {
                            try {
                                var finalStatus = new EmailStatusUpdateEventDto {
                                    JobId = (Guid)emailDto.BatchID,
                                    Status = "Completed",
                                    Message = errorMessage,
                                    //Sent = 1,
                                    Total = emailCount,
                                    FailedEmails = failedEmails,
                                    EmailTo = emailDto.EmailTo,
                                    BatchStartTime = startTime
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
            if (smtpClient != null) {
                await smtpClient.DisconnectAsync(true);
                smtpClient.Dispose();
            }
        }
    }
}