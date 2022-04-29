using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class businessactivityrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentID",
                table: "BussinessActivity",
                type: "integer",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BussinessActivity_BussinessActivity_ParentID",
                table: "BussinessActivity");

            migrationBuilder.DropIndex(
                name: "IX_BussinessActivity_ParentID",
                table: "BussinessActivity");

            migrationBuilder.DropColumn(
                name: "ParentID",
                table: "BussinessActivity");
        }
    }
}
