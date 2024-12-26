using BulkMailSender.Application.Dtos;
namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface ISaveEmailUseCase
    {
        Task<EmailDto> ExecuteAsync(EmailDto email);
    }
}