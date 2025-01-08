using BulkMailSender.Application.Dtos.UserDtos;

namespace BulkMailSender.Application.UseCases.Identity.interfaces {
    public interface IResendEmailConfirmationUseCase {
        Task<ResultDto> ExecuteAsync(UserDto user, string baseUri);
    }
}