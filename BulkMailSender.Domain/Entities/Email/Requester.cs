namespace BulkMailSender.Domain.Entities.Email
{
    public class Requester
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public Guid MailServerId { get; set; }
        // Navigation property to the associated Host entity
        public MailServer Server { get; set; } = null!;
        public bool HasValidServer()
        {
            return Server != null && !string.IsNullOrWhiteSpace(Server.ServerName) && Server.Port !=0;
        }
    }
}
