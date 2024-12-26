using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkMailSender.Infrastructure.Common.Models.Email {

    [Table("Attachment")]
    public class AttachmentEntity {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [StringLength(150)]
        public string Name { get; set; } = null!;

        public byte[] Content { get; set; } = null!;
        public virtual ICollection<EmailAttachmentEntity> EmailAttachments { get; set; } = new List<EmailAttachmentEntity>();

    }

}
