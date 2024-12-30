namespace BulkMailSender.Application.Dtos {
    public class MailServerDto {
        public Guid Id { get; set; }          // Primary key for Host
        public string ServerName { get; set; }   // SMTP server hostname (e.g., smtp.example.com)
        public int Port { get; set; }          // Port number for SMTP
        public bool IsSecure { get; set; }
    }
}
