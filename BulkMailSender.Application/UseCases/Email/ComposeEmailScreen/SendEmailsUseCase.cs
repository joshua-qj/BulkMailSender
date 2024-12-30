using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Interfaces;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Domain.Entities.Email;
using EmailSender.UseCases.EmailCompaigns.ComposeEmailScreen;
using System;
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
            ISignalRNotificationService notificationService) {
            _mapper = mapper;
            _emailRepository = emailRepository;
            _emailSenderService = emailSenderService;
            _saveEmailUseCase = saveEmailUseCase;
            _updateEmailStatusUseCase = updateEmailStatusUseCase;
            _notificationService = notificationService;
        }
        public async Task ExecuteAsync(IEnumerable<EmailDto> emails) {
            string errorMessage = string.Empty;
            Domain.Entities.Email.Email email = new Domain.Entities.Email.Email();
            foreach (var emailDto in emails) {
                try {
                    email = _mapper.Map<Domain.Entities.Email.Email>(emailDto);

                    try {
                        var requester = await _emailRepository.GetRequesterByIdAsync(emailDto.RequesterID);
                        email.SetRequester(requester);
                    }
                    catch (Exception requesterEx) {
                        errorMessage = requesterEx.Message;
                        //continue; // Skip this email
                    }
                    var emailDtoSave = _mapper.Map<EmailDto>(email);
                    var savedEmailDtoResult = await _saveEmailUseCase.ExecuteAsync(emailDtoSave);

                    email.Id = savedEmailDtoResult.Id;
                    if (!email.IsValid()) {
                        errorMessage = "Invalid email data.";
                        continue; // Skip invalid email
                    }

                    try {
                        var result = await _emailSenderService.SendAsync(email);
                        email.UpdateDeliveryStatus(result.IsSuccess ? null : result.ErrorMessage);
                        emailDtoSave = _mapper.Map<EmailDto>(email);
                        await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, result.ErrorMessage);

                    }
                    catch (Exception sendEx) {
                        errorMessage = sendEx.Message;
                    }
                }
                catch (Exception ex) {
                    errorMessage = ex.Message;
                }
                finally {
                    if (emailDto.BatchID.HasValue) {
                        await _notificationService.NotifyEmailStatusAsync(emailDto.BatchID.Value, emailDto.Id, "Failed", errorMessage);
                    }
                }
            }
        }
    }
}
