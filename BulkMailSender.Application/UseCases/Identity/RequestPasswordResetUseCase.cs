using AutoMapper;
using BulkMailSender.Application.Dtos;
using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.Email;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace BulkMailSender.Application.UseCases.Identity {
    public class RequestPasswordResetUseCase : IRequestPasswordResetUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IMapper _mapper;
        private readonly IUpdateEmailStatusUseCase _updateEmailStatusUseCase;
        private readonly IEmailSenderService _emailSenderService;

        public RequestPasswordResetUseCase(IUserRepository userRepository,
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

        public async Task<ResultDto> ExecuteAsync(string email, string baseUrl) {
            var user = await _userRepository.GetUserByUsernameAsync(email);
            if (user == null) {
                return ResultDto.Failure("User not found.");
            }
            Result confirmResult = await _authRepository.IsEmailConfirmedAsync(user.Id);


            if (!confirmResult.IsSuccess)
                return ResultDto.Failure("User email is not confirmed.");


            try {
                string token = await _authRepository.GeneratePasswordResetTokenAsync(user.Id);
                if (string.IsNullOrWhiteSpace(token)) {
                    return ResultDto.Failure("An error occurred while generating the password reset token.");
                }
                var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var userIdEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id.ToString()));
                var resetLink = $"{baseUrl}Account/ResetPassword?userId={userIdEncoded}&Code={tokenEncoded}";

                Domain.Entities.Email.Email emailDomain = new Domain.Entities.Email.Email();

                var emailDto = new EmailDto {
                    EmailFrom = "joshua.qj@hotmail.com.au",
                    DisplayName = "GroupEmail Password Reset",
                    Id = Guid.NewGuid(),
                    EmailTo = email,
                    Subject = "Reset your GroupEmail password",
                    Body = $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(resetLink)}'>clicking here</a>.",
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
                    return ResultDto.Failure("Error Occurred While Sending Your Reset Password Mail. Please contact IT administrator.");
                }
                // Store email in database
                try {
                    emailDomain = await _emailRepository.SaveEmailAsync(emailDomain);
                }
                catch (Exception) {
                    return ResultDto.Failure("Error Occurred While Sending Your Reset Password Mail. Please contact IT administrator.");
                }

                //await _emailRepository.SaveEmailAsync(email);
                var sendEmailResult = await _emailSenderService.SendAsync(emailDomain);
                emailDomain.UpdateDeliveryStatus(sendEmailResult.IsSuccess ? null : sendEmailResult.ErrorMessage);
                EmailDto emailDtoSave = _mapper.Map<EmailDto>(emailDomain);
                await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, sendEmailResult.ErrorMessage);

                if (sendEmailResult.IsSuccess == true) {
                    return ResultDto.Success();
                } else {
                    return ResultDto.Failure("Error Occurred While Sending Your Reset Password Mail. Please contact IT administrator.");
                }

            }
            catch (Exception ex) {
                // Log the exception if necessary
                // _logger.LogError(ex, "An error occurred while sending the password reset link");

                return ResultDto.Failure("An error occurred while sending the password reset link.");
            }
        }
    }
}
