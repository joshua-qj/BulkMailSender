using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces.CommonService;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using MailKit.Net.Smtp;
using System.Collections.Concurrent;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen {
    public class SendEmailsUseCase : ISendEmailsUseCase {
        private readonly IMapper _mapper;
        private readonly IEmailRepository _emailRepository;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ISaveEmailUseCase _saveEmailUseCase;
        private readonly IUpdateEmailStatusUseCase _updateEmailStatusUseCase;
        private readonly ISignalRNotificationService _notificationService;
        private readonly ISmtpClientPoolFactory _smtpClientPoolFactory;

        public SendEmailsUseCase(
            IMapper mapper,
            IEmailRepository emailRepository,
            IEmailSenderService emailSenderService,
            ISaveEmailUseCase saveEmailUseCase,
            IUpdateEmailStatusUseCase updateEmailStatusUseCase,
            IServiceProvider serviceProvider,
            ISignalRNotificationService notificationService,
            ISmtpClientPoolFactory smtpClientPoolFactory) {
            _mapper = mapper;
            _emailRepository = emailRepository;
            _emailSenderService = emailSenderService;
            _saveEmailUseCase = saveEmailUseCase;
            _updateEmailStatusUseCase = updateEmailStatusUseCase;
            _notificationService = notificationService;
            _smtpClientPoolFactory = smtpClientPoolFactory;
        }
        public async Task ExecuteAsync(IEnumerable<EmailDto> emails, DateTime? startTime, int? emailCount, RequesterDto? requesterDto, ConcurrentDictionary<Guid, EmailFailureRecord> failedEmails = null) {
            if (emails == null || !emails.Any()) {
                failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                    Email = string.Empty,
                    ErrorMessage = "Can not find an email address."
                });
                return;
            }
            ISmtpClientPool smtpClientPool = null;
            failedEmails ??= new ConcurrentDictionary<Guid, EmailFailureRecord>();
            if (requesterDto != null) {
                 smtpClientPool = _smtpClientPoolFactory.CreatePool(
                         requesterDto.Server.ServerName,
                         requesterDto.Server.Port,
                         requesterDto.LoginName,
                         requesterDto.Password
                     );
            }


            //await smtpClientPool.InitializePoolAsync();

            await Parallel.ForEachAsync(emails, new ParallelOptions { MaxDegreeOfParallelism = 8 }, async (emailDto, cancellationToken) => {
                string errorMessage = string.Empty;
                try {
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
                        errorMessage = "Set requester failed.";
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
                    if (savedEmailDtoResult == null) {
                        errorMessage = $"Save email {emailDtoSave.EmailFrom} to database failed";
                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                            Email = emailDto.EmailTo,
                            ErrorMessage = errorMessage
                        });
                        return;
                    }

                    email.Id = savedEmailDtoResult.Id;
                    if (!email.IsValid()) {
                        errorMessage = "Invalid email data. can not be sent out.";
                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                            Email = emailDto.EmailTo,
                            ErrorMessage = errorMessage
                        });
                        return;
                    }

                    SmtpClient? smtpClient = null;
                    try {
                        smtpClient = smtpClientPool != null ? await smtpClientPool.GetClientAsync() : null;
                       // Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} received smtpClient HashCode {smtpClient?.GetHashCode()}");
                        var result = await _emailSenderService.SendAsync(email, smtpClient);
                        email.UpdateDeliveryStatus(result.IsSuccess ? null : result.ErrorMessage);
                        await _updateEmailStatusUseCase.ExecuteAsync(_mapper.Map<EmailDto>(email), result.ErrorMessage);
                    }
                    catch (Exception sendEx) {
                        errorMessage = "Send email failed.";
                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                            Email = emailDto.EmailTo,
                            ErrorMessage = errorMessage
                        });
                        return;
                    }
                    finally {
                        if (smtpClient is not null) {
                            smtpClientPool?.ReturnClient(smtpClient);
                        }
                    }
                }
                catch (Exception ex) {
                    errorMessage = "An unexpected error occurred while sending the email.";
                    failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
                        Email = emailDto.EmailTo,
                        ErrorMessage = errorMessage
                    });
                }
                finally {
                    if (emailDto.BatchID.HasValue) {
                        try {
                            var finalStatus = new EmailStatusUpdateEventDto {
                                JobId = emailDto.BatchID.Value,
                                Status = "Completed",
                                Message = errorMessage,
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
                }
            });

            smtpClientPool?.Dispose();
        }



        //    public async Task ExecuteAsync(IEnumerable<EmailDto> emails, DateTime? startTime, int? emailCount, RequesterDto? requesterDto, ConcurrentDictionary<Guid, EmailFailureRecord>? failedEmails = null) {
        //        if (emails == null || !emails.Any()) return;
        //        var semaphore = new SemaphoreSlim(8,8);
        //        //SemaphoreSlim semaphore = new SemaphoreSlim(1, 8); // Limit concurrency to 4 tasks
        //        var tasks = new List<Task>();
        //        failedEmails ??= new ConcurrentDictionary<Guid, EmailFailureRecord>();
        //        // Create SmtpClient only if RequesterDto is provided
        //        MailKit.Net.Smtp.SmtpClient? smtpClient = null;
        //        //using MailKit.Net.Smtp.SmtpClient smtpClient = new MailKit.Net.Smtp.SmtpClient();
        //        //if (requesterDto != null) {
        //        //    smtpClient = new MailKit.Net.Smtp.SmtpClient();
        //        //    await smtpClient.ConnectAsync(requesterDto.Server.ServerName, requesterDto.Server.Port, SecureSocketOptions.StartTls);
        //        //    await smtpClient.AuthenticateAsync(requesterDto.LoginName, requesterDto.Password);

        //        //}
        //        if (requesterDto == null) { return; }
        //        using var smtpClientPool = _smtpClientPoolFactory.CreatePool(

        //                requesterDto.Server.ServerName,
        //                requesterDto.Server.Port,
        //                requesterDto.LoginName,
        //                requesterDto.Password
        //            );
        //        //await smtpClientPool.InitializePoolAsync();
        //        foreach (var emailDto in emails) {
        //            // Create tasks without blocking the loop
        //            tasks.Add(Task.Run(async () => {
        //                await semaphore.WaitAsync(); // Limit concurrency

        //                string errorMessage = string.Empty;

        //                try {
        //                    // Map EmailDto to Email entity
        //                    Domain.Entities.Email.Email email = _mapper.Map<Domain.Entities.Email.Email>(emailDto);

        //                    // Validate and set requester
        //                    try {
        //                        var requester = await _emailRepository.GetRequesterByIdAsync(emailDto.RequesterID);

        //                        if (requester is null) {
        //                            errorMessage = "Cannot find requester.";
        //                            failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
        //                                Email = emailDto.EmailTo,
        //                                ErrorMessage = errorMessage
        //                            });
        //                            return;
        //                        }
        //                        email.SetRequester(requester);
        //                    }
        //                    catch (Exception requesterEx) {
        //                        errorMessage = requesterEx.Message;
        //                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
        //                            Email = emailDto.EmailTo,
        //                            ErrorMessage = errorMessage
        //                        });
        //                        return; // Skip this email
        //                    }

        //                    // Save email and update ID
        //                    var emailDtoSave = _mapper.Map<EmailDto>(email);
        //                    //   var saveEmailUseCase1 = _serviceProvider.GetRequiredService<ISaveEmailUseCase>();
        //                    var savedEmailDtoResult = await _saveEmailUseCase.ExecuteAsync(emailDtoSave);
        //                    if (savedEmailDtoResult == null) {
        //                        return;
        //                    }
        //                    email.Id = savedEmailDtoResult.Id;

        //                    if (!email.IsValid()) {
        //                        errorMessage = "Invalid email data.";
        //                        // failedEmails.Add(new KeyValuePair<string, string>(emailDto.EmailTo, errorMessage));
        //                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
        //                            Email = emailDto.EmailTo,
        //                            ErrorMessage = errorMessage
        //                        });
        //                        return;
        //                    }

        //                    var smtpClient1 = await smtpClientPool.GetClientAsync();
        //                    // Send email
        //                    try {
        //                        //  Console.WriteLine($"Thread {Task.CurrentId} enters inside sendemailsusecase section at {DateTime.Now:HH:mm:ss.fff}");
        //                        Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} received smtpClient HashCode {smtpClient1?.GetHashCode()}");

        //                        //smtpClient
        //                        var result = await _emailSenderService.SendAsync(email, smtpClient1);
        //                        email.UpdateDeliveryStatus(result.IsSuccess ? null : result.ErrorMessage);

        //                        // Update email status
        //                        emailDtoSave = _mapper.Map<EmailDto>(email);
        //                        await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, result.ErrorMessage);


        //                    }
        //                    catch (Exception sendEx) {
        //                        // failedEmails.Add(new KeyValuePair<string, string>(emailDto.EmailTo, sendEx.Message));
        //                        //  failedEmails.TryAdd(emailDto.EmailTo, sendEx.Message);
        //                        errorMessage = sendEx.Message;
        //                        failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
        //                            Email = emailDto.EmailTo,
        //                            ErrorMessage = errorMessage
        //                        });
        //                        return;
        //                    }
        //                    finally { smtpClientPool.ReturnClient(smtpClient1); };

        //                }
        //                catch (Exception ex) {
        //                    // failedEmails.Add(new KeyValuePair<string, string>(emailDto.EmailTo, ex.Message));
        //                    //failedEmails.TryAdd(emailDto.EmailTo, ex.Message);
        //                    errorMessage = ex.Message;
        //                    failedEmails.TryAdd(Guid.NewGuid(), new EmailFailureRecord {
        //                        Email = emailDto.EmailTo,
        //                        ErrorMessage = errorMessage
        //                    });
        //                    return;
        //                }
        //                finally {

        //                    // Notify email status
        //                    if (emailDto.BatchID.HasValue) {
        //                        try {
        //                            var finalStatus = new EmailStatusUpdateEventDto {
        //                                JobId = (Guid)emailDto.BatchID,
        //                                Status = "Completed",
        //                                Message = errorMessage,
        //                                //Sent = 1,
        //                                Total = emailCount,
        //                                FailedEmails = failedEmails,
        //                                EmailTo = emailDto.EmailTo,
        //                                BatchStartTime = startTime
        //                            };
        //                            await _notificationService.NotifyEmailStatusAsync(emailDto.BatchID.Value, finalStatus);

        //                        }
        //                        catch (Exception ex) {
        //                            Console.WriteLine($"Failed to notify email status: {ex.Message}");
        //                        }
        //                    }

        //                    semaphore.Release(); // Release semaphore
        //                }
        //            }));
        //        }

        //        // Wait for all tasks to complete
        //        await Task.WhenAll(tasks);
        //        smtpClientPool.Dispose();
        //        if (smtpClient != null) {
        //            //await smtpClient.DisconnectAsync(true);
        //            //Console.WriteLine($"Disconnect smtpClient HashCode {smtpClient?.GetHashCode().ToString()}  at {DateTime.Now:HH:mm:ss.fff}");

        //            //smtpClient?.Dispose();
        //        }
        //    }
        //}
    }
}