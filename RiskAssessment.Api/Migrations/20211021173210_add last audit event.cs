using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RiskAssessment.Api.Migrations
{
    public partial class addlastauditevent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObjectCode",
                table: "SCORE_BOARD",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "AuditDate",
                table: "ASSESSMENT_RESULT",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastAudit",
                table: "ASSESSMENT_RESULT",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastRiskLevel",
                table: "ASSESSMENT_RESULT",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectCode",
                table: "SCORE_BOARD");

            migrationBuilder.DropColumn(
                name: "AuditDate",
                table: "ASSESSMENT_RESULT");

            migrationBuilder.DropColumn(
                name: "LastAudit",
                table: "ASSESSMENT_RESULT");

            migrationBuilder.DropColumn(
                name: "LastRiskLevel",
                table: "ASSESSMENT_RESULT");
        }
    }
}
