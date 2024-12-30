using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Entities.Email {
    [Table("Status")]
    public class StatusEntity {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty("Status")]
        public virtual ICollection<EmailEntity> Emails { get; set; } = new List<EmailEntity>();
    }
}
