using BulkMailSender.Domain.Entities.Email;
using BulkMailSender.Domain.Enums;
using BulkMailSender.Domain.ValueObjects;

namespace BulkMailSender.Domain.Services
{
    public class EmailCreationService
    {
        public Email CreateEmail(
            string subject,
            string body,
            Requester requester,
            EmailAddress emailFrom,
           EmailAddress emailTo,
    IEnumerable<Attachment>? attachments = null,
    IEnumerable<ValueObjects.InlineResource>? inlineResources = null)
        {
            if (requester == null)
                throw new ArgumentException("Requester is required.");

            if (!requester.HasValidServer())
                throw new InvalidOperationException("Requester must belong to a valid host.");

            Email email= new Email
            {
                Id = Guid.NewGuid(),
                Subject = subject,
                Body = body,
                EmailFrom = emailFrom,
                EmailTo = emailTo,
                Requester = requester,
                RequestedDate = DateTime.UtcNow,
                StatusId = (int)Status.Ready
            }; 
            if (attachments != null)
                foreach (var attachment in attachments)
                    email.AddAttachment(attachment);

            if (inlineResources != null)
                foreach (var resource in inlineResources)
                    email.AddInlineResource(resource);

            return email;
        }
    }
}
