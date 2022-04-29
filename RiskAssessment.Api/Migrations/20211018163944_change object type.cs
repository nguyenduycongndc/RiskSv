using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class changeobjecttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FacilityId",
                table: "SCORE_BOARD",
                newName: "ObjectId");

            migrationBuilder.RenameColumn(
                name: "Facility",
                table: "SCORE_BOARD",
                newName: "ObjectName");

            migrationBuilder.AlterColumn<string>(
                name: "ApplyFor",
                table: "SCORE_BOARD_ISSUE",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplyFor",
                table: "SCORE_BOARD",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyFor",
                table: "SCORE_BOARD");

            migrationBuilder.RenameColumn(
                name: "ObjectName",
                table: "SCORE_BOARD",
                newName: "Facility");

            migrationBuilder.RenameColumn(
                name: "ObjectId",
                table: "SCORE_BOARD",
                newName: "FacilityId");

            migrationBuilder.AlterColumn<int>(
                name: "ApplyFor",
                table: "SCORE_BOARD_ISSUE",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
