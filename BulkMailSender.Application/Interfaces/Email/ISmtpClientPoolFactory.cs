namespace BulkMailSender.Application.Interfaces.Email {
    public interface ISmtpClientPoolFactory {
        ISmtpClientPool CreatePool(string serverName, int port, string username, string password, int maxPoolSize = 8);
    }
}