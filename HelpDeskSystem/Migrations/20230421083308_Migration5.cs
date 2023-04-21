using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "messageId",
                table: "EmailInfos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "idConfigEmail",
                table: "EmailInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "idConfigEmail",
                table: "EmailInfos");

            migrationBuilder.AlterColumn<string>(
                name: "messageId",
                table: "EmailInfos",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
