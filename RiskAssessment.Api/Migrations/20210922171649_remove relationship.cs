using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class removerelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentID",
                table: "AuditFacility");

            migrationBuilder.DropIndex(
                name: "IX_AuditFacility_ParentID",
                table: "AuditFacility");

            //migrationBuilder.DropColumn(
            //    name: "AuditFacilityID",
            //    table: "AuditFacility");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "AuditFacility",
                newName: "ParentId");

            migrationBuilder.AddColumn<string>(
                name: "ParentName",
                table: "AuditFacility",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentName",
                table: "AuditFacility");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "AuditFacility",
                newName: "ParentID");

            //migrationBuilder.AddColumn<int>(
            //    name: "AuditFacilityID",
            //    table: "AuditFacility",
            //    type: "integer",
            //    nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditFacility_ParentID",
                table: "AuditFacility",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentID",
                table: "AuditFacility",
                column: "ParentID",
                principalTable: "AuditFacility",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
