using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IResetPasswordUseCase {
        Task<ResultDto> ExecuteAsync(string email, string token, string newPassword, string confirmPassword);
    }
}