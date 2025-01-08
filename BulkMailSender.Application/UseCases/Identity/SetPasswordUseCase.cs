using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class SetPasswordUseCase : ISetPasswordUseCase {
        private readonly IAuthRepository _authRepository;
        private readonly IUserRepository _userRepository;

        public SetPasswordUseCase(IAuthRepository authRepository,
            IUserRepository userRepository) {
            _authRepository = authRepository;
            _userRepository = userRepository;
        }

        public async Task<ResultDto> ExecuteAsync(Guid userId, string newPassword, string confirmPassword) {
            // Validate passwords match
            if (newPassword != confirmPassword) {
                return ResultDto.Failure("The password and confirmation password do not match.");
            }

            // Validate user exists
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null) {
                return ResultDto.Failure("User not found.");
            }

            // Check if user already has a password
            var hasPassword = await _authRepository.CheckUserHasPassword(userId);
            if (hasPassword) {
                return ResultDto.Failure("User already has a password set.");
            }

            // Add the new password
            Result addResult = await _authRepository.AddPasswordAsync(userId, newPassword);
            if (!addResult.IsSuccess) {
                return ResultDto.Failure(addResult.Errors.ToArray());
            }
            return ResultDto.Success();
        }
    }
}