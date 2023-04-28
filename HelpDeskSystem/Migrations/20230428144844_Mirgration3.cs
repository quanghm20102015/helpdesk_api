using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HelpDeskSystem.Migrations
{
    /// <inheritdoc />
    public partial class Mirgration3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "idCompany",
                table: "EmailInfos",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeamAgents",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    idAgent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamAgents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    idCompany = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamAgents");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropColumn(
                name: "idCompany",
                table: "EmailInfos");
        }
    }
}
