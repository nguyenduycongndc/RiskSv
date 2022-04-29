using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RiskAssessment.Api.Migrations
{
    public partial class updatedbrelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentId",
                table: "AuditFacility");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "RiskId",
                table: "RiskAssessmentScale");

            migrationBuilder.DropColumn(
                name: "ObjectId",
                table: "AuditFacility");

            migrationBuilder.RenameColumn(
                name: "FormulaId",
                table: "RiskIssue",
                newName: "FormulaID");

            migrationBuilder.RenameColumn(
                name: "ObjectId",
                table: "RiskIssue",
                newName: "RiskIssueID");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "AuditFacility",
                newName: "ParentID");

            migrationBuilder.RenameIndex(
                name: "IX_AuditFacility_ParentId",
                table: "AuditFacility",
                newName: "IX_AuditFacility_ParentID");

            migrationBuilder.AlterColumn<int>(
                name: "MethodId",
                table: "RiskIssue",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplyForID",
                table: "RiskIssue",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RiskIssueID",
                table: "RiskAssessmentScale",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ParentID",
                table: "AuditFacility",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ObjectClassID",
                table: "AuditFacility",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Formula",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true)
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
                name: "IX_RiskIssue_FormulaID",
                table: "RiskIssue",
                column: "FormulaID");

            migrationBuilder.CreateIndex(
                name: "IX_RiskIssue_RiskIssueID",
                table: "RiskIssue",
                column: "RiskIssueID");

            migrationBuilder.CreateIndex(
                name: "IX_RiskAssessmentScale_RiskIssueID",
                table: "RiskAssessmentScale",
                column: "RiskIssueID");

            migrationBuilder.CreateIndex(
                name: "IX_AuditFacility_ObjectClassID",
                table: "AuditFacility",
                column: "ObjectClassID");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentID",
                table: "AuditFacility",
                column: "ParentID",
                principalTable: "AuditFacility",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditFacility_SystemCategory_ObjectClassID",
                table: "AuditFacility",
                column: "ObjectClassID",
                principalTable: "SystemCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskAssessmentScale_RiskIssue_RiskIssueID",
                table: "RiskAssessmentScale",
                column: "RiskIssueID",
                principalTable: "RiskIssue",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_Formula_FormulaID",
                table: "RiskIssue",
                column: "FormulaID",
                principalTable: "Formula",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_RiskIssue_RiskIssueID",
                table: "RiskIssue",
                column: "RiskIssueID",
                principalTable: "RiskIssue",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RiskIssue_SystemCategory_ApplyForID",
                table: "RiskIssue",
                column: "ApplyForID",
                principalTable: "SystemCategory",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentID",
                table: "AuditFacility");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditFacility_SystemCategory_ObjectClassID",
                table: "AuditFacility");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskAssessmentScale_RiskIssue_RiskIssueID",
                table: "RiskAssessmentScale");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_Formula_FormulaID",
                table: "RiskIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_RiskIssue_RiskIssueID",
                table: "RiskIssue");

            migrationBuilder.DropForeignKey(
                name: "FK_RiskIssue_SystemCategory_ApplyForID",
                table: "RiskIssue");

            migrationBuilder.DropTable(
                name: "Formula");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_ApplyForID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_FormulaID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskIssue_RiskIssueID",
                table: "RiskIssue");

            migrationBuilder.DropIndex(
                name: "IX_RiskAssessmentScale_RiskIssueID",
                table: "RiskAssessmentScale");

            migrationBuilder.DropIndex(
                name: "IX_AuditFacility_ObjectClassID",
                table: "AuditFacility");

            migrationBuilder.DropColumn(
                name: "ApplyForID",
                table: "RiskIssue");

            migrationBuilder.DropColumn(
                name: "RiskIssueID",
                table: "RiskAssessmentScale");

            migrationBuilder.DropColumn(
                name: "ObjectClassID",
                table: "AuditFacility");

            migrationBuilder.RenameColumn(
                name: "FormulaID",
                table: "RiskIssue",
                newName: "FormulaId");

            migrationBuilder.RenameColumn(
                name: "RiskIssueID",
                table: "RiskIssue",
                newName: "ObjectId");

            migrationBuilder.RenameColumn(
                name: "ParentID",
                table: "AuditFacility",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditFacility_ParentID",
                table: "AuditFacility",
                newName: "IX_AuditFacility_ParentId");

            migrationBuilder.AlterColumn<int>(
                name: "MethodId",
                table: "RiskIssue",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "RiskIssue",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RiskId",
                table: "RiskAssessmentScale",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "AuditFacility",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ObjectId",
                table: "AuditFacility",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditFacility_AuditFacility_ParentId",
                table: "AuditFacility",
                column: "ParentId",
                principalTable: "AuditFacility",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
