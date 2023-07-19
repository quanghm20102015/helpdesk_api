using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration49 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "language",
                table: "Accounts",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "numberDay",
                table: "Accounts",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "language",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "numberDay",
                table: "Accounts");
        }
    }
}
