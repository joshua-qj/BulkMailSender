using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BulkMailSender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InlineResource",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InlineResource", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailServer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServerName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    IsSecure = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailServer", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Requester",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MailServerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requester", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Requester_MailServer_MailServerId",
                        column: x => x.MailServerId,
                        principalTable: "MailServer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Email",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailFrom = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmailTo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsBodyHtml = table.Column<bool>(type: "bit", nullable: true),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BatchId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Email", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Email_Requester_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "Requester",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Email_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmailAttachment",
                columns: table => new
                {
                    EmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailAttachment", x => new { x.EmailId, x.AttachmentId });
                    table.ForeignKey(
                        name: "FK_EmailAttachment_Attachment_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "Attachment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailAttachment_Email_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Email",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailInlineResource",
                columns: table => new
                {
                    EmailId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InlineResourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailInlineResource", x => new { x.EmailId, x.InlineResourceId });
                    table.ForeignKey(
                        name: "FK_EmailInlineResource_Email_EmailId",
                        column: x => x.EmailId,
                        principalTable: "Email",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailInlineResource_InlineResource_InlineResourceId",
                        column: x => x.InlineResourceId,
                        principalTable: "InlineResource",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "MailServer",
                columns: new[] { "Id", "IsSecure", "Port", "ServerName" },
                values: new object[] { new Guid("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1"), true, 587, "smtp-relay.brevo.com" });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Ready" },
                    { 2, "Delivered" },
                    { 3, "Undelivered" },
                    { 4, "Retrying" },
                    { 6, "Canceled" },
                    { 7, "InvalidRecipient" }
                });

            migrationBuilder.InsertData(
                table: "Requester",
                columns: new[] { "Id", "LoginName", "MailServerId", "Password" },
                values: new object[] { new Guid("a6e9e69e-3af3-43c3-a6e9-775f751f3659"), "joshua.qj@hotmail.com", new Guid("a7dc4d29-69fa-4d8f-92aa-df9b3076aad1"), "YCNDXj6t7LfMc1yW" });

            migrationBuilder.CreateIndex(
                name: "IX_Email_RequesterId",
                table: "Email",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Email_StatusId",
                table: "Email",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailAttachment_AttachmentId",
                table: "EmailAttachment",
                column: "AttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailInlineResource_InlineResourceId",
                table: "EmailInlineResource",
                column: "InlineResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Requester_MailServerId",
                table: "Requester",
                column: "MailServerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailAttachment");

            migrationBuilder.DropTable(
                name: "EmailInlineResource");

            migrationBuilder.DropTable(
                name: "Attachment");

            migrationBuilder.DropTable(
                name: "Email");

            migrationBuilder.DropTable(
                name: "InlineResource");

            migrationBuilder.DropTable(
                name: "Requester");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "MailServer");
        }
    }
}
