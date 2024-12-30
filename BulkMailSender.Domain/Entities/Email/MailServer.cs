namespace BulkMailSender.Domain.Entities.Email
{
    public class MailServer
    {
        public Guid Id { get; set; }          // Primary key for Host
        public string ServerName { get; set; }   // SMTP server hostname (e.g., smtp.example.com)
        public int Port { get; set; }          // Port number for SMTP
        public bool IsSecure { get; set; }    // Whether SSL is enabled for SMTP
                                              // Constructor to enforce initialization
        public MailServer(string serverName, int port, bool isSecure) {
            if (string.IsNullOrWhiteSpace(serverName))
                throw new ArgumentException("Server name cannot be null or empty.", nameof(serverName));

            if (port <= 0 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Port must be between 1 and 65535.");

            ServerName = serverName;
            Port = port;
            IsSecure = isSecure;
        }        
        // Validate server connectivity
        public bool ValidateConnectivity() {
            // Simulate checking connectivity (actual implementation may involve SMTP library)
            return !string.IsNullOrWhiteSpace(ServerName) && Port > 0 && Port <= 65535;
        }
        // Collection of requesters associated with this host
        //public ICollection<Requester> Requesters { get; set; } = new List<Requester>();
    }
}
