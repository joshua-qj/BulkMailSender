using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class LogoutUseCase : ILogoutUseCase {
        private readonly IAuthRepository _authRepository;
        public LogoutUseCase(IAuthRepository authRepository) {
            _authRepository = authRepository;
        }

        public async Task<ResultDto> ExecuteAsync() {
            Result result = await _authRepository.SignOutAsync();

            if (result.IsSuccess) {
                return ResultDto.Success();
            } else {
                return ResultDto.Failure(result.Errors.ToArray());
            }
        }

    }
}
