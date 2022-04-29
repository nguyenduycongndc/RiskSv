using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class UpdateChangeRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApply",
                table: "SCORE_BOARD_ISSUE",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplyFor",
                table: "RATING_SCALE",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApply",
                table: "SCORE_BOARD_ISSUE");

            migrationBuilder.DropColumn(
                name: "ApplyFor",
                table: "RATING_SCALE");
        }
    }
}
