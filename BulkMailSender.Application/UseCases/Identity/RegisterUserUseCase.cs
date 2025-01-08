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
    public class RegisterUserUseCase : IRegisterUserUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IEmailRepository _emailRepository;
        private readonly IUpdateEmailStatusUseCase _updateEmailStatusUseCase;
        private readonly IEmailSenderService _emailSenderService;
        private readonly IMapper _mapper;

        public RegisterUserUseCase(IUserRepository userRepository,
            IAuthRepository authRepository,
            IEmailRepository emailRepository,
            IUpdateEmailStatusUseCase updateEmailStatusUseCase,
            IEmailSenderService emailSenderService,
            IMapper mapper) {
            _userRepository = userRepository;
            _authRepository = authRepository;
            _emailRepository = emailRepository;
            _updateEmailStatusUseCase = updateEmailStatusUseCase;
            _emailSenderService = emailSenderService;
            _mapper = mapper;
        }
        public async Task<ResultDto> ExecuteAsync(string email, string password, string confirmPassword, string? baseUri) {
            if (password != confirmPassword) {
                return ResultDto.Failure("The password and confirmation password do not match.");
            }

            var user = new UserDto { Id = Guid.NewGuid(), UserName = email, Email = email };

            try {
                // Register user
                var checkEmailExisted = await _userRepository.GetUserByUsernameAsync(user.UserName);
                if (checkEmailExisted != null) {
                    return ResultDto.Failure("User email address already existed.");
                }
                var domianUser = _mapper.Map<User>(user);
                var registerResult = await _userRepository.RegisterUserAsync(domianUser, password);
                if (!registerResult.IsSuccess) {
                    return ResultDto.Failure("User creation failed.");
                }

                // Generate confirmation token
                var token = await _authRepository.GenerateEmailConfirmationTokenAsync(user.Id);

                // Encode token and user ID for URL
                var tokenEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var userIdEncoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(user.Id.ToString()));
                var confirmationLink = $"{baseUri}Account/ConfirmEmail?userId={userIdEncoded}&Code={tokenEncoded}";
                Domain.Entities.Email.Email emailDomain = new Domain.Entities.Email.Email();
                // Create email message
                var emailDto = new EmailDto {

                    EmailFrom ="joshua.qj@hotmail.com.au",
                    DisplayName = "GroupEmail confirmation",
                    Id = Guid.NewGuid(),
                    EmailTo = email,
                    Subject = "Confirm your GroupEmail register email",
                    Body = $"This is an automatically generated email, please do not reply. Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.For security purposes, the link will expire in 12 hours",
                    IsBodyHtml = true,
                    RequestedDate = DateTime.UtcNow,
                    RequesterID = Guid.Parse("a6e9e69e-3af3-43c3-a6e9-775f751f3659"),
                    //"a6e9e69e - 3af3 - 43c3 - a6e9 - 775f751f3659''
                    StatusId = 1
                };
                try {
                    var requester = await _emailRepository.GetRequesterByIdAsync(emailDto.RequesterID);
                    emailDomain =_mapper.Map<Domain.Entities.Email.Email>(emailDto);
                    emailDomain.SetRequester(requester);
                }
                catch (Exception ) {
                    return ResultDto.Failure("User created, but confirmation email failed. Please contact IT administrator.");
                }
                // Store email in database
                try {
                    emailDomain = await _emailRepository.SaveEmailAsync(emailDomain);
                }
                catch (Exception) {
                    return ResultDto.Failure("User created, but confirmation email failed. Please contact IT administrator.");
                }

                //await _emailRepository.SaveEmailAsync(email);
                var sendEmailResult = await _emailSenderService.SendAsync(emailDomain);

                emailDomain.UpdateDeliveryStatus(sendEmailResult.IsSuccess ? null : sendEmailResult.ErrorMessage);
                EmailDto emailDtoSave = _mapper.Map<EmailDto>(emailDomain);
                await _updateEmailStatusUseCase.ExecuteAsync(emailDtoSave, sendEmailResult.ErrorMessage);

                if (sendEmailResult.IsSuccess == true) {
                    return ResultDto.Success();
                } else {
                    return ResultDto.Failure("User created, but confirmation email failed. Please contact IT administrator.");
                }

            }
            catch (Exception ex) {
                // Log the exception if necessary
                // _logger.LogError(ex, "An error occurred during user registration");


                return ResultDto.Failure("An error occurred during registration.");
            }
        }
    }
}

