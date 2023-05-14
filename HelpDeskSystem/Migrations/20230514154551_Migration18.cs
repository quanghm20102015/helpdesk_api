using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration18 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "dateDelete",
                table: "EmailInfos",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "idUserDelete",
                table: "EmailInfos",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "isDelete",
                table: "EmailInfos",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "dateDelete",
                table: "EmailInfos");

            migrationBuilder.DropColumn(
                name: "idUserDelete",
                table: "EmailInfos");

            migrationBuilder.DropColumn(
                name: "isDelete",
                table: "EmailInfos");
        }
    }
}
