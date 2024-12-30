namespace BulkMailSender.Application.Dtos {
    public class RequesterDto {
        public Guid Id { get; set; }
        public string LoginName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid MailServerId { get; set; } 
        public MailServerDto Server { get; set; } = null!;
    }
}
