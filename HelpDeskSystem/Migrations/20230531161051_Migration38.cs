using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration38 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "extension",
                table: "EmailInfoAttachs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "EmailInfoAttachs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sizeText",
                table: "EmailInfoAttachs",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "extension",
                table: "EmailInfoAttachs");

            migrationBuilder.DropColumn(
                name: "name",
                table: "EmailInfoAttachs");

            migrationBuilder.DropColumn(
                name: "sizeText",
                table: "EmailInfoAttachs");
        }
    }
}
