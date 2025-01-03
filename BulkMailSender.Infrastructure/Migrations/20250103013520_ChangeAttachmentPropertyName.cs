using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BulkMailSender.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAttachmentPropertyName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Attachment",
                newName: "FileName");

         
        
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FileName",
                table: "Attachment",
                newName: "FileName");

         
        }
    }
}
