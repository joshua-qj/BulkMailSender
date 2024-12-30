namespace BulkMailSender.Blazor.ViewModels {
    public class MailServerViewModel {
        public Guid Id { get; set; }          // Primary key for Host
        public string ServerName { get; set; }   // SMTP server hostname (e.g., smtp.example.com)
        public int Port { get; set; }          // Port number for SMTP
        public bool IsSecure { get; set; }
    }
}
