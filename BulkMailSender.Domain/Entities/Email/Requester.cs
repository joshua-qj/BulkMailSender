namespace BulkMailSender.Domain.Entities.Email
{
    public class Requester {
        public Guid Id { get; set; }
        public string LoginName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid MailServerId { get; set; }
        // Navigation property to the associated Host entity
        public MailServer Server { get; set; } = null!;
        public bool HasValidServer() {
            return Server != null && !string.IsNullOrWhiteSpace(Server.ServerName) && Server.Port != 0;
        }    // Constructor to enforce initialization
        public Requester(string loginName, string password, MailServer server) {
            if (string.IsNullOrWhiteSpace(loginName))
                throw new ArgumentException("Login name cannot be null or empty.", nameof(loginName));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            if (server == null || !server.ValidateConnectivity())
                throw new InvalidOperationException("Invalid mail server.");

            LoginName = loginName;
            Password = password;
            Server = server;
            MailServerId = server.Id;
        }    
    }
}
