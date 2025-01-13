using BulkMailSender.Application.Dtos;

namespace BulkMailSender.Application.UseCases.Email.ViewEmailScreen.Interfaces {
    public interface IGetEmailByIdUseCase {
        Task<EmailDto?> ExecuteAsync(Guid emailId);
    }
}