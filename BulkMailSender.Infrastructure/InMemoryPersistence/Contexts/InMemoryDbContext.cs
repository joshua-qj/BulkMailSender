﻿using BulkMailSender.Infrastructure.Common.Entities.Email;
using Microsoft.EntityFrameworkCore;

namespace BulkMailSender.Infrastructure.InMemoryPersistence.Contexts {
    public class InMemoryDbContext : DbContext {
        public InMemoryDbContext(DbContextOptions<InMemoryDbContext> options)
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

            // Configure additional properties, if necessary
            modelBuilder.Entity<AttachmentEntity>()
                .Property(a => a.FileName)
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

            // Seeding MailServer
            modelBuilder.Entity<MailServerEntity>().HasData(
                new MailServerEntity {
                    Id = Guid.Parse("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1"),
                    ServerName = "smtp-relay.brevo.com",
                    Port = 587,
                    IsSecure = true
                }
            );

            // Seeding Status
            modelBuilder.Entity<StatusEntity>().HasData(
                new StatusEntity { Id = 1, Name = "Ready" },
                new StatusEntity { Id = 2, Name = "Delivered" },
                new StatusEntity { Id = 3, Name = "Undelivered" },
                new StatusEntity { Id = 4, Name = "Retrying" },
                new StatusEntity { Id = 6, Name = "Canceled" },
                new StatusEntity { Id = 7, Name = "InvalidRecipient" }
            );

            // Seeding Requester
            modelBuilder.Entity<RequesterEntity>().HasData(
                new RequesterEntity {
                    Id = Guid.Parse("a6e9e69e-3af3-43c3-a6e9-775f751f3659"),
                    LoginName = "joshua.qj@hotmail.com",
                    Password = "YCNDXj6t7LfMc1yW",
                    MailServerId = Guid.Parse("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1")
                }
            );
        }

        public static InMemoryDbContext CreateInMemoryDbContext() {
            var options = new DbContextOptionsBuilder<InMemoryDbContext>()
                .UseInMemoryDatabase("InMemoryBulkMailSenderDb") // Define the database name
                .Options;

            return new InMemoryDbContext(options);
        }

        // Add DbSet for all your entities
        public DbSet<AttachmentEntity> Attachments { get; set; }
        public DbSet<EmailAttachmentEntity> EmailAttachments { get; set; }
        public DbSet<EmailEntity> Emails { get; set; }
        public DbSet<InlineResourceEntity> InlineResources { get; set; }
        public DbSet<MailServerEntity> MailServers { get; set; }
        public DbSet<RequesterEntity> Requesters { get; set; }
        public DbSet<StatusEntity> Statuses { get; set; }
    }
}
