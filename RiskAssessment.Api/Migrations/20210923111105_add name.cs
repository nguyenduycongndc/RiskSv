using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class addname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActivityName",
                table: "AuditProcess",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FacilityName",
                table: "AuditProcess",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActivityName",
                table: "AuditProcess");

            migrationBuilder.DropColumn(
                name: "FacilityName",
                table: "AuditProcess");
        }
    }
}
