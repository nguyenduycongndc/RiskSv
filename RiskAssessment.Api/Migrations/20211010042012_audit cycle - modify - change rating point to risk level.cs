using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class auditcyclemodifychangeratingpointtorisklevel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RatingPointName",
                table: "AUDIT_CYCLE",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RatingPointName",
                table: "AUDIT_CYCLE");
        }
    }
}
