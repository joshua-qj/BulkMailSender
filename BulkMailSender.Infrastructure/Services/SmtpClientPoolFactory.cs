using BulkMailSender.Application.Interfaces.Email;

namespace BulkMailSender.Infrastructure.Services {
    public class SmtpClientPoolFactory : ISmtpClientPoolFactory {
        public ISmtpClientPool CreatePool(string serverName, int port, string username, string password, int maxPoolSize = 8) {
            return new SmtpClientPool(maxPoolSize, serverName, port, username, password);
        }
    }
}
