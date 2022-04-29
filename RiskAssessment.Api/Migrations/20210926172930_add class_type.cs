using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class addclass_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassTypeID",
                table: "RiskIssue",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_ClassTypeID",
                table: "RiskIssue",
                column: "ClassTypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_SystemCategory_ClassTypeID",
                table: "RiskIssue",
                column: "ClassTypeID",
                principalTable: "SystemCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_SystemCategory_ClassTypeID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_ClassTypeID",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "ClassTypeID",
                table: "RiskIssue");
        }
    }
}
