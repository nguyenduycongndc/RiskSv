using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class fixrelationship_add_fak_e : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_AuditFacility_AuditFacility_AuditFacilityID",
            //    table: "AuditFacility");

            //migrationBuilder.DropIndex(
            //    name: "IX_AuditFacility_AuditFacilityID",
            //    table: "AuditFacility");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentID",
                table: "AuditFacility");

            migrationBuilder.DropIndex(
                name: "IX_AuditFacility_ParentID",
                table: "AuditFacility");

            //migrationBuilder.CreateIndex(
            //    name: "IX_AuditFacility_AuditFacilityID",
            //    table: "AuditFacility",
            //    column: "AuditFacilityID");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_AuditFacility_AuditFacility_AuditFacilityID",
            //    table: "AuditFacility",
            //    column: "AuditFacilityID",
            //    principalTable: "AuditFacility",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Restrict);
        }
    }
}
