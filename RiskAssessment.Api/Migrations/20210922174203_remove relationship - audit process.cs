using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class removerelationshipauditprocess : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditProcess_AuditActivity_ActivityID",
                table: "AuditProcess");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditProcess_AuditFacility_FacilityID",
                table: "AuditProcess");

            migrationBuilder.DropIndex(
                name: "IX_AuditProcess_ActivityID",
                table: "AuditProcess");

            migrationBuilder.DropIndex(
                name: "IX_AuditProcess_FacilityID",
                table: "AuditProcess");

            migrationBuilder.RenameColumn(
                name: "FacilityID",
                table: "AuditProcess",
                newName: "FacilityId");

            migrationBuilder.RenameColumn(
                name: "ActivityID",
                table: "AuditProcess",
                newName: "ActivityId");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityId",
                table: "AuditProcess",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ActivityId",
                table: "AuditProcess",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FacilityId",
                table: "AuditProcess",
                newName: "FacilityID");

            migrationBuilder.RenameColumn(
                name: "ActivityId",
                table: "AuditProcess",
                newName: "ActivityID");

            migrationBuilder.AlterColumn<int>(
                name: "FacilityID",
                table: "AuditProcess",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "ActivityID",
                table: "AuditProcess",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "IX_AuditProcess_ActivityID",
                table: "AuditProcess",
                column: "ActivityID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditProcess_FacilityID",
                table: "AuditProcess",
                column: "FacilityID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditProcess_AuditActivity_ActivityID",
                table: "AuditProcess",
                column: "ActivityID",
                principalTable: "AuditActivity",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditProcess_AuditFacility_FacilityID",
                table: "AuditProcess",
                column: "FacilityID",
                principalTable: "AuditFacility",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
