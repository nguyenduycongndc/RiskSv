using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace RiskAssessment.Api.Migrations
{
    public partial class scorebỏad : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SCORE_BOARD",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssessmentStageId = table.Column<int>(type: "integer", nullable: false),
                    FacilityId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false),
                    StageValue = table.Column<int>(type: "integer", nullable: true),
                    Facility = table.Column<string>(type: "text", nullable: true),
                    StateInfo = table.Column<string>(type: "text", nullable: true),
                    Point = table.Column<double>(type: "double precision", nullable: true),
                    RiskLevel = table.Column<string>(type: "text", nullable: true),
                    Target = table.Column<string>(type: "text", nullable: true),
                    MainProcess = table.Column<string>(type: "text", nullable: true),
                    ITSystem = table.Column<string>(type: "text", nullable: true),
                    Project = table.Column<string>(type: "text", nullable: true),
                    Outsourcing = table.Column<string>(type: "text", nullable: true),
                    Supplier = table.Column<string>(type: "text", nullable: true),
                    InternalRegulations = table.Column<string>(type: "text", nullable: true),
                    LawRegulations = table.Column<string>(type: "text", nullable: true),
                    AttachFile = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCORE_BOARD", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SCORE_BOARD_ISSUE",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScoreBoardId = table.Column<int>(type: "integer", nullable: false),
                    IssueId = table.Column<int>(type: "integer", nullable: false),
                    IssueParentId = table.Column<int>(type: "integer", nullable: true),
                    Point = table.Column<int>(type: "integer", nullable: true),
                    Condition = table.Column<string>(type: "text", nullable: true),
                    RiskValue = table.Column<double>(type: "double precision", nullable: true),
                    Proportion = table.Column<int>(type: "integer", nullable: true),
                    ProportionModify = table.Column<int>(type: "integer", nullable: true),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Code = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<bool>(type: "boolean", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserCreate = table.Column<int>(type: "integer", nullable: true),
                    LastModified = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ModifiedBy = table.Column<int>(type: "integer", nullable: true),
                    DomainId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SCORE_BOARD_ISSUE", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SCORE_BOARD");

            migrationBuilder.DropTable(
                name: "SCORE_BOARD_ISSUE");
        }
    }
}
