using BulkMailSender.Application.Dtos.UserDtos;
using BulkMailSender.Application.Interfaces.User;
using BulkMailSender.Application.UseCases.Identity.interfaces;
using BulkMailSender.Domain.Entities.Identity;

namespace BulkMailSender.Application.UseCases.Identity {
    public class ChangePasswordUseCase : IChangePasswordUseCase {
        private readonly IAuthRepository _authRepository;

        public ChangePasswordUseCase(IAuthRepository authRepository) {
            _authRepository = authRepository;
        }

        public async Task<ResultDto> ExecuteAsync(Guid userId, string oldPassword, string newPassword) {
            Result result = await _authRepository.ChangePasswordAsync(userId, oldPassword, newPassword);

            if (!result.IsSuccess) {
                return ResultDto.Failure(string.Join(",", result.Errors));
            }
            return ResultDto.Success();
        }
    }
}