using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface ISetPasswordUseCase {
        Task<ResultDto> ExecuteAsync(Guid userId, string newPassword, string confirmPassword);
    }
}