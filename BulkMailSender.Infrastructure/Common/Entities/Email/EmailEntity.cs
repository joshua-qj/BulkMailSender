using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Entities.Email {
    [Table("Email")]
    public class EmailEntity {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [StringLength(100)]
        public string EmailFrom { get; set; } = null!;

        [StringLength(100)]
        public string? DisplayName { get; set; }

        [StringLength(100)]
        public string EmailTo { get; set; } = null!;

        [StringLength(250)]
        public string Subject { get; set; }

        public string Body { get; set; }

        [Column("IsBodyHtml")]
        public bool IsBodyHtml { get; set; }

        [Column("RequesterId")]
        public Guid RequesterId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? RequestedDate { get; set; }

        [Column("StatusId")]
        public int StatusId { get; set; }

        [StringLength(1000)]
        public string? ErrorMessage { get; set; }

        [Column("BatchId")]
        public Guid? BatchId { get; set; }

        [Column("UserId")]
        public Guid? UserId { get; set; }
        public virtual ICollection<EmailAttachmentEntity> EmailAttachments { get; set; } = new List<EmailAttachmentEntity>();
        public virtual ICollection<EmailInlineResourceEntity> EmailInlineResources { get; set; } = new List<EmailInlineResourceEntity>();

        [ForeignKey("RequesterId")]
        [InverseProperty("Emails")]
        public virtual RequesterEntity Requester { get; set; } = null!;

        [ForeignKey("StatusId")]
        [InverseProperty("Emails")]
        public virtual StatusEntity? Status { get; set; }
    }
}

