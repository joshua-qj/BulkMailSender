using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IToggleUserStatusUseCase {
        Task<ResultDto> ExecuteAsync(Guid userId);
    }
}