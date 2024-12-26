using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Models.Email {
    [Table("EmailInlineResource")]
    public class EmailInlineResourceEntity {
        [Column("EmailId")]
        public Guid EmailId { get; set; } // Foreign Key to EmailEntity

        [Column("InlineResourceId")]
        public Guid InlineResourceId { get; set; } // Foreign Key to LinkedResourceEntity

        [ForeignKey("EmailId")]
        [InverseProperty("EmailInlineResources")]
        public virtual EmailEntity Email { get; set; } = null!;

        [ForeignKey("InlineResourceId")]
        [InverseProperty("EmailInlineResources")]
        public virtual InlineResourceEntity InlineResource { get; set; } = null!;
    }
}
