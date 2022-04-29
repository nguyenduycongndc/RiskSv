using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class adddomainidfordatapermission : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_RiskIssue_RiskIssueID",
                table: "RiskIssue");

            migrationBuilder.RenameColumn(
                name: "RiskIssueID",
                table: "RiskIssue",
                newName: "ParentID");

            migrationBuilder.RenameIndex(
                name: "IX_RiskIssue_RiskIssueID",
                table: "RiskIssue",
                newName: "IX_RiskIssue_ParentID");

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "SystemCategory",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "RiskIssue",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "RiskAssessmentScale",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "RatingScale",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "Formula",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "BussinessActivity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "AuditProcess",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "AuditFacility",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "AuditCycle",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DomainId",
                table: "AuditActivity",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_RiskIssue_ParentID",
                table: "RiskIssue",
                column: "ParentID",
                principalTable: "RiskIssue",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_RiskIssue_ParentID",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "SystemCategory");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "RiskAssessmentScale");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "RatingScale");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "Formula");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "BussinessActivity");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "AuditProcess");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "AuditFacility");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "AuditCycle");

            migrationBuilder.DropColumn(
                name: "DomainId",
                table: "AuditActivity");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "RiskIssue",
                newName: "RiskIssueID");

            migrationBuilder.RenameIndex(
                name: "IX_RiskIssue_ParentID",
                table: "RiskIssue",
                newName: "IX_RiskIssue_RiskIssueID");

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_RiskIssue_RiskIssueID",
                table: "RiskIssue",
                column: "RiskIssueID",
                principalTable: "RiskIssue",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
