using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class removerelationship_2509 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BussinessActivity_BussinessActivity_ParentID",
                table: "BussinessActivity");

            migrationBuilder.DropIndex(
                name: "IX_BussinessActivity_ParentID",
                table: "BussinessActivity");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "BussinessActivity",
                newName: "ParentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "BussinessActivity",
                newName: "ParentID");

            migrationBuilder.CreateIndex(
                name: "IX_BussinessActivity_ParentID",
                table: "BussinessActivity",
                column: "ParentID");

            migrationBuilder.AddForeignKey(
                name: "FK_BussinessActivity_BussinessActivity_ParentID",
                table: "BussinessActivity",
                column: "ParentID",
                principalTable: "BussinessActivity",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
