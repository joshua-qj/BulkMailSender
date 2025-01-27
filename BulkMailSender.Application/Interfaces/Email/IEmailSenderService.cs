namespace BulkMailSender.Application.Interfaces.Email {
    public interface IEmailSenderService {
        Task<(bool IsSuccess, string ErrorMessage)> SendAsync(Domain.Entities.Email.Email email, MailKit.Net.Smtp.SmtpClient? smtpClient = null);
    }
}