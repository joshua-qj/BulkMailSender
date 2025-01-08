using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace BulkMailSender.Application.UseCases.Identity {
    public class ResendEmailConfirmationUseCase : IResendEmailConfirmationUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;
        private readonly IUpdateEmailStatusUseCase _updateEmailStatusUseCase;
        private readonly IEmailSenderService _emailSenderService;

        public ResendEmailConfirmationUseCase(IUserRepository userRepository,
            IAuthRepository authRepository,
            IEmailRepository emailRepository,
            IMapper mapper,
            IUpdateEmailStatusUseCase updateEmailStatusUseCase,
            IEmailSenderService emailSenderService) {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _emailRepository = emailRepository;
            _mapper = mapper;
            _updateEmailStatusUseCase = updateEmailStatusUseCase;
            _emailSenderService = emailSenderService;
        }
        public async Task<ResultDto> ExecuteAsync(UserDto user, string baseUri) {
            if (user == null) {
                return ResultDto.Failure("No user found with the given email address.");
            }

            // Generate confirmation token
            var token = await _authRepository.GenerateEmailConfirmationTokenAsync(user.Id);
            var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var userIdEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id.ToString()));
            var confirmationLink = $"{baseUri}Account/ConfirmEmail?userId={userIdEncoded}&Code={tokenEncoded}";

            Domain.Entities.Email.Email emailDomain = new Domain.Entities.Email.Email();

            var emailDto = new EmailDto {
                EmailFrom = "joshua.qj@hotmail.com.au",
                DisplayName = "GroupEmail confirmation",
                Id = Guid.NewGuid(),
                EmailTo = user.Email,
                Subject = "Confirm your GroupEmail register email",
                Body = $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.",
                IsBodyHtml = true,
                RequestedDate = DateTime.UtcNow,
                RequesterID = Guid.Parse("a6e9e69e-3af3-43c3-a6e9-775f751f3659"),
                StatusId = 1
            };
            try {
                var requester = await _emailRepository.GetRequesterByIdAsync(emailDto.RequesterID);
                emailDomain = _mapper.Map<Domain.Entities.Email.Email>(emailDto);
                emailDomain.SetRequester(requester);
            }
            catch (Exception) {
                return ResultDto.Failure("Error Occurred While Resending Confirmation Mail. Please contact IT administrator.");
            }
            // Store email in database
            try {
                emailDomain = await _emailRepository.SaveEmailAsync(emailDomain);
            }
            catch (Exception) {
                return ResultDto.Failure("Error Occurred While Resending Confirmation Mail. Please contact IT administrator.");
            }
            var sendEmailResult = await _emailSenderService.SendAsync(emailDomain);
            emailDomain.UpdateDeliveryStatus(sendEmailResult.IsSuccess ? null : sendEmailResult.ErrorMessage);
            EmailDto emailDtoSave = _mapper.Map<EmailDto>(emailDomain);
            await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, sendEmailResult.ErrorMessage);

            if (sendEmailResult.IsSuccess == true) {
                return ResultDto.Success();
            } else {
                return ResultDto.Failure("Error Occurred While Resending Confirmation Mail. Please contact IT administrator.");
            }
        }
    }
}