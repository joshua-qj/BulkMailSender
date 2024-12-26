using BulkMailSender.Infrastructure.Common.Models.Email;
using BulkMailSender.Infrastructure.Common.Models.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.SQLServerPersistence.Contexts {
    public class SqlServerDbContext : DbContext {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // EmailAttachment relationships
            modelBuilder.Entity<EmailAttachmentEntity>()
                .HasKey(ea => new { ea.EmailId, ea.AttachmentId }); // Composite key

            modelBuilder.Entity<EmailAttachmentEntity>()
                .HasOne(ea => ea.Email)
                .WithMany(e => e.EmailAttachments)
                .HasForeignKey(ea => ea.EmailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailAttachmentEntity>()
                .HasOne(ea => ea.Attachment)
                .WithMany(a => a.EmailAttachments)
                .HasForeignKey(ea => ea.AttachmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // EmailInlineResource relationships
            modelBuilder.Entity<EmailInlineResourceEntity>()
                .HasKey(eir => new { eir.EmailId, eir.InlineResourceId }); // Composite key

            modelBuilder.Entity<EmailInlineResourceEntity>()
                .HasOne(eir => eir.Email)
                .WithMany(e => e.EmailInlineResources)
                .HasForeignKey(eir => eir.EmailId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailInlineResourceEntity>()
                .HasOne(eir => eir.InlineResource)
                .WithMany(ir => ir.EmailInlineResources)
                .HasForeignKey(eir => eir.InlineResourceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Email relationships
            modelBuilder.Entity<EmailEntity>()
                .HasOne(e => e.Requester)
                .WithMany(r => r.Emails)
                .HasForeignKey(e => e.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EmailEntity>()
                .HasOne(e => e.Status)
                .WithMany(s => s.Emails)
                .HasForeignKey(e => e.StatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Requester relationships
            modelBuilder.Entity<RequesterEntity>()
                .HasOne(r => r.MailServer)
                .WithMany(ms => ms.Requesters)
                .HasForeignKey(r => r.MailServerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Additional properties
            modelBuilder.Entity<AttachmentEntity>()
                .Property(a => a.Name)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<InlineResourceEntity>()
                .Property(ir => ir.FileName)
                .HasMaxLength(255);

            modelBuilder.Entity<InlineResourceEntity>()
                .Property(ir => ir.MimeType)
                .HasMaxLength(255);

            modelBuilder.Entity<StatusEntity>()
                .Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<RequesterEntity>()
                .Property(r => r.LoginName)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<RequesterEntity>()
                .Property(r => r.Password)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<MailServerEntity>()
                .Property(ms => ms.ServerName)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<EmailEntity>()
                .Property(e => e.EmailFrom)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<EmailEntity>()
                .Property(e => e.EmailTo)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<EmailEntity>()
                .Property(e => e.DisplayName)
                .HasMaxLength(100);

            modelBuilder.Entity<EmailEntity>()
                .Property(e => e.Subject)
                .HasMaxLength(250);

            modelBuilder.Entity<EmailEntity>()
                .Property(e => e.ErrorMessage)
                .HasMaxLength(1000);

       }

        // DbSets for all your entities
        public DbSet<AttachmentEntity> Attachments { get; set; }
        public DbSet<EmailAttachmentEntity> EmailAttachments { get; set; }
        public DbSet<EmailEntity> Emails { get; set; }
        public DbSet<InlineResourceEntity> InlineResources { get; set; }
        public DbSet<MailServerEntity> MailServers { get; set; }
        public DbSet<RequesterEntity> Requesters { get; set; }
        public DbSet<StatusEntity> Statuses { get; set; }
    }
}

