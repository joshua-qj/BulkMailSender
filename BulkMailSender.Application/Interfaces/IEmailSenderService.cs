using BulkMailSender.Domain.Entities.Email;

namespace BulkMailSender.Application.Interfaces {
    public interface IEmailSenderService {
        Task<(bool IsSuccess, string ErrorMessage)> SendAsync(Email email);
    }
}