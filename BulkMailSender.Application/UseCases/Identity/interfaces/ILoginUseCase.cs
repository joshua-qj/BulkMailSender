using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface ILoginUseCase {
        Task<ResultDto> ExecuteAsync(string email, string password, bool rememberMe);
    }
}