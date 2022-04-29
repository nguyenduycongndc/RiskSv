using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class addauditnameforscoreboard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AuditCycleName",
                table: "SCORE_BOARD",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AuditCycleName",
                table: "SCORE_BOARD");
        }
    }
}
