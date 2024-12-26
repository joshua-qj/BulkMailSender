using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkMailSender.Infrastructure.Common.Models.Email {

    [Table("Requester")]
    public class RequesterEntity {
        [Key]
        [Column("Id")]
        public Guid Id { get; set; }

        [StringLength(150)]
        public string LoginName { get; set; } = null!;

        [StringLength(50)]
        public string Password { get; set; } = null!;

        [Column("MailServerId")]
        public Guid MailServerId { get; set; }

        [InverseProperty("Requester")]
        public virtual ICollection<EmailEntity> Emails { get; set; } = new List<EmailEntity>();

        [ForeignKey("MailServerId")]
        [InverseProperty("Requesters")]
        public virtual MailServerEntity MailServer { get; set; } = null!;
    }
}
