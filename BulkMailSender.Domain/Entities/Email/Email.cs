using BulkMailSender.Domain.Enums;
using BulkMailSender.Domain.ValueObjects;

namespace BulkMailSender.Domain.Entities.Email
{
    public class Email
    {
        public Guid Id { get; set; }
        public EmailAddress EmailFrom { get; set; }
        public string? DisplayName { get; set; }
        public EmailAddress EmailTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? IsBodyHTML { get; set; }
        public Guid RequesterID { get; set; }
        public Requester Requester { get; set; }
        public DateTime RequestedDate { get; set; }
        public int StatusId { get; set; } // Maps to the enum
        public Status Status => (Status)StatusId; // Enum for code readability
        public string? ErrorMessage { get; set; }
        public Guid? BatchID { get; set; }
        public Guid UserId { get; set; }
        // Using HashSet for uniqueness
        public HashSet<ValueObjects.Attachment> Attachments { get; private set; } = new HashSet<ValueObjects.Attachment>();
        public HashSet<ValueObjects.InlineResource> InlineResources { get; private set; } = new HashSet<ValueObjects.InlineResource>();

        // Business Logic
        public bool IsValid() =>
    !string.IsNullOrWhiteSpace(EmailFrom?.Value) &&
    !string.IsNullOrWhiteSpace(EmailTo?.Value) &&
    !string.IsNullOrWhiteSpace(Subject) &&
    !string.IsNullOrWhiteSpace(Body) &&
    Requester is not null &&
    Requester.HasValidServer();
        public void SetRequester(Requester requester)
        {
            if (requester == null || !requester.HasValidServer())
                throw new InvalidOperationException("Invalid requester or host.");

            Requester = requester;
        }
        public void AddAttachment(ValueObjects.Attachment attachment)
        {
            if (Attachments.Contains(attachment))
                throw new InvalidOperationException("Duplicate attachment detected.");
            Attachments.Add(attachment);
        }

        public void AddInlineResource(ValueObjects.InlineResource resource)
        {
            if (InlineResources.Contains(resource))
                throw new InvalidOperationException("Duplicate inline resource detected.");
            InlineResources.Add(resource);
        }
      //  public void MarkAsDelivered() => StatusId = (int)Status.Delivered; 
      //public void MarkAsUndelivered(string? errorMessage)
      //  {
      //      StatusId = string.IsNullOrEmpty(errorMessage) ? (int)Status.Delivered : (int)Status.Undelivered;
      //      ErrorMessage = errorMessage;
      //  }
        public void UpdateDeliveryStatus(string? errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
            {
                StatusId = (int)Status.Delivered;
                ErrorMessage = null;
            } else
            {
                StatusId = (int)Status.Undelivered;
                ErrorMessage = errorMessage;
            }
        }
    }
}
