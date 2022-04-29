using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class rename_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskAssessmentScale_RiskIssue_RiskIssueID",
                table: "RiskAssessmentScale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Formula",
                table: "Formula");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SystemCategory",
                table: "SystemCategory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RiskIssue",
                table: "RiskIssue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RiskAssessmentScale",
                table: "RiskAssessmentScale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RatingScale",
                table: "RatingScale");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BussinessActivity",
                table: "BussinessActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditProcess",
                table: "AuditProcess");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditFacility",
                table: "AuditFacility");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditCycle",
                table: "AuditCycle");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditActivity",
                table: "AuditActivity");

            migrationBuilder.RenameTable(
                name: "Formula",
                newName: "FORMULA");

            migrationBuilder.RenameTable(
                name: "SystemCategory",
                newName: "SYSTEM_CATEGORY");

            migrationBuilder.RenameTable(
                name: "RiskIssue",
                newName: "RISK_ISSUE");

            migrationBuilder.RenameTable(
                name: "RiskAssessmentScale",
                newName: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.RenameTable(
                name: "RatingScale",
                newName: "RATING_SCALE");

            migrationBuilder.RenameTable(
                name: "BussinessActivity",
                newName: "BUSSINESS_ACTIVITY");

            migrationBuilder.RenameTable(
                name: "AuditProcess",
                newName: "AUDIT_PROCESS");

            migrationBuilder.RenameTable(
                name: "AuditFacility",
                newName: "AUDIT_FACILITY");

            migrationBuilder.RenameTable(
                name: "AuditCycle",
                newName: "AUDIT_CYCLE");

            migrationBuilder.RenameTable(
                name: "AuditActivity",
                newName: "AUDIT_ACTIVITY");

            migrationBuilder.RenameIndex(
                name: "IX_RiskAssessmentScale_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE",
                newName: "IX_RISK_ASSESSMENT_SCALE_RiskIssueID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FORMULA",
                table: "FORMULA",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SYSTEM_CATEGORY",
                table: "SYSTEM_CATEGORY",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RISK_ISSUE",
                table: "RISK_ISSUE",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RISK_ASSESSMENT_SCALE",
                table: "RISK_ASSESSMENT_SCALE",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RATING_SCALE",
                table: "RATING_SCALE",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BUSSINESS_ACTIVITY",
                table: "BUSSINESS_ACTIVITY",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_PROCESS",
                table: "AUDIT_PROCESS",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_FACILITY",
                table: "AUDIT_FACILITY",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_CYCLE",
                table: "AUDIT_CYCLE",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AUDIT_ACTIVITY",
                table: "AUDIT_ACTIVITY",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RISK_ASSESSMENT_SCALE_RISK_ISSUE_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE",
                column: "RiskIssueID",
                principalTable: "RISK_ISSUE",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RISK_ASSESSMENT_SCALE_RISK_ISSUE_RiskIssueID",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FORMULA",
                table: "FORMULA");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SYSTEM_CATEGORY",
                table: "SYSTEM_CATEGORY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RISK_ISSUE",
                table: "RISK_ISSUE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RISK_ASSESSMENT_SCALE",
                table: "RISK_ASSESSMENT_SCALE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RATING_SCALE",
                table: "RATING_SCALE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BUSSINESS_ACTIVITY",
                table: "BUSSINESS_ACTIVITY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_PROCESS",
                table: "AUDIT_PROCESS");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_FACILITY",
                table: "AUDIT_FACILITY");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_CYCLE",
                table: "AUDIT_CYCLE");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AUDIT_ACTIVITY",
                table: "AUDIT_ACTIVITY");

            migrationBuilder.RenameTable(
                name: "FORMULA",
                newName: "Formula");

            migrationBuilder.RenameTable(
                name: "SYSTEM_CATEGORY",
                newName: "SystemCategory");

            migrationBuilder.RenameTable(
                name: "RISK_ISSUE",
                newName: "RiskIssue");

            migrationBuilder.RenameTable(
                name: "RISK_ASSESSMENT_SCALE",
                newName: "RiskAssessmentScale");

            migrationBuilder.RenameTable(
                name: "RATING_SCALE",
                newName: "RatingScale");

            migrationBuilder.RenameTable(
                name: "BUSSINESS_ACTIVITY",
                newName: "BussinessActivity");

            migrationBuilder.RenameTable(
                name: "AUDIT_PROCESS",
                newName: "AuditProcess");

            migrationBuilder.RenameTable(
                name: "AUDIT_FACILITY",
                newName: "AuditFacility");

            migrationBuilder.RenameTable(
                name: "AUDIT_CYCLE",
                newName: "AuditCycle");

            migrationBuilder.RenameTable(
                name: "AUDIT_ACTIVITY",
                newName: "AuditActivity");

            migrationBuilder.RenameIndex(
                name: "IX_RISK_ASSESSMENT_SCALE_RiskIssueID",
                table: "RiskAssessmentScale",
                newName: "IX_RiskAssessmentScale_RiskIssueID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Formula",
                table: "Formula",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SystemCategory",
                table: "SystemCategory",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RiskIssue",
                table: "RiskIssue",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RiskAssessmentScale",
                table: "RiskAssessmentScale",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RatingScale",
                table: "RatingScale",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BussinessActivity",
                table: "BussinessActivity",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditProcess",
                table: "AuditProcess",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditFacility",
                table: "AuditFacility",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditCycle",
                table: "AuditCycle",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditActivity",
                table: "AuditActivity",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_RiskAssessmentScale_RiskIssue_RiskIssueID",
                table: "RiskAssessmentScale",
                column: "RiskIssueID",
                principalTable: "RiskIssue",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
