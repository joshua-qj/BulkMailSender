using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IChangePasswordUseCase {
        Task<ResultDto> ExecuteAsync(Guid userId, string oldPassword, string newPassword);
    }
}