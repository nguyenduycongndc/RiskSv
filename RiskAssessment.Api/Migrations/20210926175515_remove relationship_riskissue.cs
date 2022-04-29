using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class removerelationship_riskissue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_RiskIssue_ParentID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_ParentID",
                table: "RiskIssue");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "RiskIssue",
                newName: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "RiskIssue",
                newName: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_ParentID",
                table: "RiskIssue",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_RiskIssue_ParentID",
                table: "RiskIssue",
                column: "ParentID",
                principalTable: "RiskIssue",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
