using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface ICheckUserHasPasswordUseCase {
        Task<bool> ExecuteAsync(Guid userId);
    }
}