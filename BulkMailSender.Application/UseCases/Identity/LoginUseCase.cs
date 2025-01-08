using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class LoginUseCase : ILoginUseCase {
        private readonly IAuthRepository _authRepository;
        // private readonly ILogger<User> _logger;
        public LoginUseCase(IAuthRepository authRepository) {
            _authRepository = authRepository;
        }

        public async Task<ResultDto> ExecuteAsync(string email, string password, bool rememberMe) {
            Result result = await _authRepository.AuthenticateAsync(email, password, rememberMe);

            if (result.IsSuccess) {
                return ResultDto.Success();
            } else {
                return ResultDto.Failure(result.Errors.ToArray());
            }
        }
    }
}
