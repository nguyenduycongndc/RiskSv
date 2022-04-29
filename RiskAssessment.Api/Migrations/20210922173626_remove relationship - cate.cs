using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class removerelationshipcate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditFacility_SystemCategory_ObjectClassID",
                table: "AuditFacility");

            migrationBuilder.DropIndex(
                name: "IX_AuditFacility_ObjectClassID",
                table: "AuditFacility");

            migrationBuilder.RenameColumn(
                name: "ObjectClassID",
                table: "AuditFacility",
                newName: "ObjectClassId");

            migrationBuilder.AddColumn<string>(
                name: "ObjectClassName",
                table: "AuditFacility",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectClassName",
                table: "AuditFacility");

            migrationBuilder.RenameColumn(
                name: "ObjectClassId",
                table: "AuditFacility",
                newName: "ObjectClassID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditFacility_ObjectClassID",
                table: "AuditFacility",
                column: "ObjectClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditFacility_SystemCategory_ObjectClassID",
                table: "AuditFacility",
                column: "ObjectClassID",
                principalTable: "SystemCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
