namespace BulkMailSender.Domain.Entities.Email
{
    public class Host
    {
        public Guid Id { get; set; }          // Primary key for Host
        public string HostName { get; set; }   // SMTP server hostname (e.g., smtp.example.com)
        public int PortNumber { get; set; }          // Port number for SMTP
        public bool EnableSsl { get; set; }    // Whether SSL is enabled for SMTP

        // Collection of requesters associated with this host
        //public ICollection<Requester> Requesters { get; set; } = new List<Requester>();
    }
}
