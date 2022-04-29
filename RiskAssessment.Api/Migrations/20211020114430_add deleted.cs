using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class adddeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SYSTEM_CATEGORY",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SCORE_BOARD_ISSUE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "SCORE_BOARD",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "RISK_ISSUE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "RISK_ASSESSMENT_SCALE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "RATING_SCALE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "FORMULA",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "BUSSINESS_ACTIVITY",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AUDIT_PROCESS",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AUDIT_FACILITY",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AUDIT_CYCLE",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "AUDIT_ACTIVITY",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "ASSESSMENT_STAGE",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SYSTEM_CATEGORY");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SCORE_BOARD_ISSUE");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "SCORE_BOARD");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "RISK_ISSUE");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "RATING_SCALE");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "FORMULA");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "BUSSINESS_ACTIVITY");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AUDIT_PROCESS");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AUDIT_FACILITY");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AUDIT_CYCLE");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "AUDIT_ACTIVITY");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "ASSESSMENT_STAGE");
        }
    }
}
