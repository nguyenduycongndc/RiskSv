using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RiskAssessment.Api.Migrations
{
    public partial class removeallrelationshipriskissue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_Formula_FormulaID",
                table: "RiskIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_SystemCategory_ApplyForID",
                table: "RiskIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_SystemCategory_ClassTypeID",
                table: "RiskIssue");

            migrationBuilder.DropTable(
                name: "Formula");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_ApplyForID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_ClassTypeID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_FormulaID",
                table: "RiskIssue");

            migrationBuilder.RenameColumn(
                name: "FormulaID",
                table: "RiskIssue",
                newName: "Formula");

            migrationBuilder.RenameColumn(
                name: "ClassTypeID",
                table: "RiskIssue",
                newName: "ClassType");

            migrationBuilder.RenameColumn(
                name: "ApplyForID",
                table: "RiskIssue",
                newName: "ApplyFor");

            migrationBuilder.AddColumn<string>(
                name: "ApplyForName",
                table: "RiskIssue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassTypeName",
                table: "RiskIssue",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormulaName",
                table: "RiskIssue",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplyForName",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "ClassTypeName",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "FormulaName",
                table: "RiskIssue");

            migrationBuilder.RenameColumn(
                name: "Formula",
                table: "RiskIssue",
                newName: "FormulaID");

            migrationBuilder.RenameColumn(
                name: "ClassType",
                table: "RiskIssue",
                newName: "ClassTypeID");

            migrationBuilder.RenameColumn(
                name: "ApplyFor",
                table: "RiskIssue",
                newName: "ApplyForID");

            migrationBuilder.CreateTable(
                name: "Formula",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Formula", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_ApplyForID",
                table: "RiskIssue",
                column: "ApplyForID");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_ClassTypeID",
                table: "RiskIssue",
                column: "ClassTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_FormulaID",
                table: "RiskIssue",
                column: "FormulaID");

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_Formula_FormulaID",
                table: "RiskIssue",
                column: "FormulaID",
                principalTable: "Formula",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_SystemCategory_ApplyForID",
                table: "RiskIssue",
                column: "ApplyForID",
                principalTable: "SystemCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_SystemCategory_ClassTypeID",
                table: "RiskIssue",
                column: "ClassTypeID",
                principalTable: "SystemCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
