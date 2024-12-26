using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Models.Email {
    [Table("EmailAttachment")]

    public class EmailAttachmentEntity {

        public Guid EmailId { get; set; }

        public Guid AttachmentId { get; set; }

        // Navigation properties
        [ForeignKey(nameof(EmailId))]
        public virtual EmailEntity Email { get; set; } = null!;

        [ForeignKey(nameof(AttachmentId))]
        public virtual AttachmentEntity Attachment { get; set; } = null!;
    }
}