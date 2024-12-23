namespace BulkMailSender.Domain.Entities.Email
{
    public class Requester
    {
        public Guid Id { get; set; }
        public string LoginName { get; set; }
        public Guid HostId { get; set; }
        // Navigation property to the associated Host entity
        public Host Host { get; set; }
        public bool HasValidHost()
        {
            return Host != null && !string.IsNullOrWhiteSpace(Host.HostName) && Host.PortNumber !=0;
        }
    }
}
