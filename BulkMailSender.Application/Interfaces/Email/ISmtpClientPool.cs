using MailKit.Net.Smtp;

namespace BulkMailSender.Application.Interfaces.Email {
    public interface ISmtpClientPool : IDisposable {
        Task<SmtpClient> GetClientAsync();
        Task InitializePoolAsync();
        void ReturnClient(SmtpClient client);
    }
}