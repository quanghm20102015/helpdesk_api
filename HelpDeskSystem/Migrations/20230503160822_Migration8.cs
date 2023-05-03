using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idLabel",
                table: "Contacts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idLabel",
                table: "Contacts");
        }
    }
}
