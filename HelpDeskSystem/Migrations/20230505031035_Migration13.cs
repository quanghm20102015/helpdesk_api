using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idEmailInfo",
                table: "Csats");

            migrationBuilder.AddColumn<string>(
                name: "idGuIdEmailInfo",
                table: "Csats",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idGuIdEmailInfo",
                table: "Csats");

            migrationBuilder.AddColumn<int>(
                name: "idEmailInfo",
                table: "Csats",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
