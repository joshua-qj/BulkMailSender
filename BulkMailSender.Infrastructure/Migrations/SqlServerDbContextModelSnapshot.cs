﻿// <auto-generated />
using System;
using BulkMailSender.Infrastructure.SQLServerPersistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BulkMailSender.Infrastructure.Migrations
{
    [DbContext(typeof(SqlServerDbContext))]
    partial class SqlServerDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.AttachmentEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("Attachment");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailAttachmentEntity", b =>
                {
                    b.Property<Guid>("EmailId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AttachmentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("EmailId", "AttachmentId");

                    b.HasIndex("AttachmentId");

                    b.ToTable("EmailAttachment");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<Guid?>("BatchId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("BatchId");

                    b.Property<string>("Body")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DisplayName")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("EmailFrom")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("EmailTo")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("ErrorMessage")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<bool>("IsBodyHtml")
                        .HasColumnType("bit")
                        .HasColumnName("IsBodyHtml");

                    b.Property<DateTime?>("RequestedDate")
                        .HasColumnType("datetime");

                    b.Property<Guid>("RequesterId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("RequesterId");

                    b.Property<int>("StatusId")
                        .HasColumnType("int")
                        .HasColumnName("StatusId");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("UserId");

                    b.HasKey("Id");

                    b.HasIndex("RequesterId");

                    b.HasIndex("StatusId");

                    b.ToTable("Email");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailInlineResourceEntity", b =>
                {
                    b.Property<Guid>("EmailId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("EmailId");

                    b.Property<Guid>("InlineResourceId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("InlineResourceId");

                    b.HasKey("EmailId", "InlineResourceId");

                    b.HasIndex("InlineResourceId");

                    b.ToTable("EmailInlineResource");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.InlineResourceEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<byte[]>("Content")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("FileName")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("MimeType")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("InlineResource");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.MailServerEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<bool>("IsSecure")
                        .HasColumnType("bit");

                    b.Property<int>("Port")
                        .HasColumnType("int");

                    b.Property<string>("ServerName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.HasKey("Id");

                    b.ToTable("MailServer");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1"),
                            IsSecure = true,
                            Port = 587,
                            ServerName = "smtp-relay.brevo.com"
                        });
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.RequesterEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id");

                    b.Property<string>("LoginName")
                        .IsRequired()
                        .HasMaxLength(150)
                        .HasColumnType("nvarchar(150)");

                    b.Property<Guid>("MailServerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("MailServerId");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("MailServerId");

                    b.ToTable("Requester");

                    b.HasData(
                        new
                        {
                            Id = new Guid("a6e9e69e-3af3-43c3-a6e9-775f751f3659"),
                            LoginName = "joshua.qj@hotmail.com",
                            MailServerId = new Guid("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1"),
                            Password = "YCNDXj6t7LfMc1yW"
                        });
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.StatusEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("Status");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Ready"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Delivered"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Undelivered"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Retrying"
                        },
                        new
                        {
                            Id = 6,
                            Name = "Canceled"
                        },
                        new
                        {
                            Id = 7,
                            Name = "InvalidRecipient"
                        });
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailAttachmentEntity", b =>
                {
                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.AttachmentEntity", "Attachment")
                        .WithMany("EmailAttachments")
                        .HasForeignKey("AttachmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.EmailEntity", "Email")
                        .WithMany("EmailAttachments")
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attachment");

                    b.Navigation("Email");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailEntity", b =>
                {
                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.RequesterEntity", "Requester")
                        .WithMany("Emails")
                        .HasForeignKey("RequesterId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.StatusEntity", "Status")
                        .WithMany("Emails")
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Requester");

                    b.Navigation("Status");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailInlineResourceEntity", b =>
                {
                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.EmailEntity", "Email")
                        .WithMany("EmailInlineResources")
                        .HasForeignKey("EmailId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.InlineResourceEntity", "InlineResource")
                        .WithMany("EmailInlineResources")
                        .HasForeignKey("InlineResourceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Email");

                    b.Navigation("InlineResource");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.RequesterEntity", b =>
                {
                    b.HasOne("BulkMailSender.Infrastructure.Common.Entities.Email.MailServerEntity", "MailServer")
                        .WithMany("Requesters")
                        .HasForeignKey("MailServerId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MailServer");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.AttachmentEntity", b =>
                {
                    b.Navigation("EmailAttachments");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.EmailEntity", b =>
                {
                    b.Navigation("EmailAttachments");

                    b.Navigation("EmailInlineResources");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.InlineResourceEntity", b =>
                {
                    b.Navigation("EmailInlineResources");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.MailServerEntity", b =>
                {
                    b.Navigation("Requesters");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.RequesterEntity", b =>
                {
                    b.Navigation("Emails");
                });

            modelBuilder.Entity("BulkMailSender.Infrastructure.Common.Entities.Email.StatusEntity", b =>
                {
                    b.Navigation("Emails");
                });
#pragma warning restore 612, 618
        }
    }
}
