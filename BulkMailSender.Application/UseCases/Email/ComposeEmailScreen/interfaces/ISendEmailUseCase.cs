using BulkMailSender.Application.Dtos;
namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {

    public interface ISendEmailUseCase {
        Task<(bool IsSuccess, string ErrorMessage)> ExecuteAsync(EmailDto email);
    }
}