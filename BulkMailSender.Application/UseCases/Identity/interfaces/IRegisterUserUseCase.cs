using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IRegisterUserUseCase {
        Task<ResultDto> ExecuteAsync(string email, string password, string confirmPassword, string? baseUri);
    }
}