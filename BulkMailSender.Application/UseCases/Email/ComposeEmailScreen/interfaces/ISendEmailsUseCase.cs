using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.UseCases.Email.ComposeEmailScreen.interfaces {
    public interface ISendEmailsUseCase {
        Task ExecuteAsync(IEnumerable<EmailDto> emails);
    }
}