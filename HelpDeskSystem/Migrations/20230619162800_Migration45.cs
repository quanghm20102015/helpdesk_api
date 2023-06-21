using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Migration45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAgents");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.AddColumn<bool>(
                name: "autoAssign",
                table: "Teams",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isDelete",
                table: "Teams",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<List<int>>(
                name: "listAgent",
                table: "Teams",
                type: "integer[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "autoAssign",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "isDelete",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "listAgent",
                table: "Teams");

            migrationBuilder.CreateTable(
                name: "GroupAgents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    idGroup = table.Column<int>(type: "integer", nullable: false),
                    idUser = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAgents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    autoAssign = table.Column<bool>(type: "boolean", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    idCompany = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.id);
                });
        }
    }
}
