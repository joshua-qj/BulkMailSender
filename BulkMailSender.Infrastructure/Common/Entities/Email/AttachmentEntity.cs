using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Entities.Email {

    [Table("Attachment")]
    public class AttachmentEntity {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [StringLength(150)]
        public string FileName { get; set; } = null!;

        public byte[] Content { get; set; } = null!;
        public virtual ICollection<EmailAttachmentEntity> EmailAttachments { get; set; } = new List<EmailAttachmentEntity>();

    }

}
