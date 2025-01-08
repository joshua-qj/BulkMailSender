using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IRequestPasswordResetUseCase {
        Task<ResultDto> ExecuteAsync(string email, string baseUrl);
    }
}