using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Entities.Email {
    [Table("MailServer")]
    public class MailServerEntity {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [StringLength(150)]
        public string ServerName { get; set; } = null!;

        public int Port { get; set; }

        public bool IsSecure { get; set; }

        [InverseProperty("MailServer")]
        public virtual ICollection<RequesterEntity> Requesters { get; set; } = new List<RequesterEntity>();
    }
}
