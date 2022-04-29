using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class riskscalechangerequirement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaxFunction",
                table: "RATING_SCALE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MinFunction",
                table: "RATING_SCALE",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxFunction",
                table: "RATING_SCALE");

            migrationBuilder.DropColumn(
                name: "MinFunction",
                table: "RATING_SCALE");
        }
    }
}
