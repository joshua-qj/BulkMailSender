using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;

namespace BulkMailSender.Application.UseCases.Identity {
    public class ResetPasswordUseCase : IResetPasswordUseCase {
        private readonly IUserRepository _userRepository;
        private readonly IAuthRepository _authRepository;

        public ResetPasswordUseCase(IUserRepository userRepository, IAuthRepository authRepository) {
            _userRepository = userRepository;
            _authRepository = authRepository;
        }

        public async Task<ResultDto> ExecuteAsync(string email, string token, string newPassword, string confirmPassword) {
            if (newPassword != confirmPassword) {
                return ResultDto.Failure("The password and confirmation password do not match.");
            }
            var user = await _userRepository.GetUserByUsernameAsync(email);
            if (user == null) {
                return ResultDto.Failure("User not found.");
            }

            var resetResult = await _authRepository.ResetPasswordAsync(user.Id, token, newPassword);
            if (resetResult.IsSuccess) {
                return ResultDto.Success();
            }
            return ResultDto.Failure(resetResult.Errors.ToArray());

        }
    }
}