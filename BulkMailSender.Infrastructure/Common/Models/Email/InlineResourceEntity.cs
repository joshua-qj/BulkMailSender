using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BulkMailSender.Infrastructure.Common.Models.Email {
    public class InlineResourceEntity {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; } // Unique identifier for the resource

        public byte[] Content { get; set; } // The content of the linked resource (could be an image, document, etc.)

        [StringLength(255)]
        public string? FileName { get; set; } // Optional: The file name of the resource

        [StringLength(255)]
        public string? MimeType { get; set; } // The MIME type (e.g., image/png, application/pdf, etc.)
        [Column(TypeName = "datetime2")]
        public virtual ICollection<EmailInlineResourceEntity> EmailInlineResources { get; set; } = new List<EmailInlineResourceEntity>(); // Many-to-many relationship
    }
}