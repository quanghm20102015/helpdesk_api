using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "from",
                table: "EmailInfos",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "EmailInfos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "messageId",
                table: "EmailInfos",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date",
                table: "EmailInfos");

            migrationBuilder.DropColumn(
                name: "messageId",
                table: "EmailInfos");

            migrationBuilder.AlterColumn<string>(
                name: "from",
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
