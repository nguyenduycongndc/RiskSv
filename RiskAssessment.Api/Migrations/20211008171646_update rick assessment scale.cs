using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class updaterickassessmentscale : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RISK_ASSESSMENT_SCALE_RISK_ISSUE_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropIndex(
                name: "IX_RISK_ASSESSMENT_SCALE_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.RenameColumn(
                name: "RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "RiskIssueId");

            migrationBuilder.RenameColumn(
                name: "MinCondition",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "UpperCondition");

            migrationBuilder.RenameColumn(
                name: "MaxCondition",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "LowerCondition");

            migrationBuilder.AddColumn<string>(
                name: "LowerConditionName",
                table: "RISK_ASSESSMENT_SCALE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskIssueCode",
                table: "RISK_ASSESSMENT_SCALE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiskIssueName",
                table: "RISK_ASSESSMENT_SCALE",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpperConditionName",
                table: "RISK_ASSESSMENT_SCALE",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LowerConditionName",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropColumn(
                name: "RiskIssueCode",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropColumn(
                name: "RiskIssueName",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropColumn(
                name: "UpperConditionName",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.RenameColumn(
                name: "RiskIssueId",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "RiskIssueID");

            migrationBuilder.RenameColumn(
                name: "UpperCondition",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "MinCondition");

            migrationBuilder.RenameColumn(
                name: "LowerCondition",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "MaxCondition");

            migrationBuilder.CreateIndex(
                name: "IX_RISK_ASSESSMENT_SCALE_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE",
                column: "RiskIssueID");

            migrationBuilder.AddForeignKey(
                name: "FK_RISK_ASSESSMENT_SCALE_RISK_ISSUE_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE",
                column: "RiskIssueID",
                principalTable: "RISK_ISSUE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
