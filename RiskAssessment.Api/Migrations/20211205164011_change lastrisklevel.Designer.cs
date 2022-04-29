﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.Api.Migrations
{
    [DbContext(typeof(AuditSysContext))]
    [Migration("20211205164011_change lastrisklevel")]
    partial class changelastrisklevel
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AssessmentResult", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AssessmentStatus")
                        .HasColumnType("integer");

                    b.Property<bool>("Audit")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("AuditDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("AuditReason")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastAudit")
                        .HasColumnType("timestamp without time zone");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LastRiskLevel")
                        .HasColumnType("text");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("PassAuditReason")
                        .HasColumnType("text");

                    b.Property<int>("RiskLevel")
                        .HasColumnType("integer");

                    b.Property<int>("RiskLevelChange")
                        .HasColumnType("integer");

                    b.Property<string>("RiskLevelChangeName")
                        .HasColumnType("text");

                    b.Property<int>("ScoreBoardId")
                        .HasColumnType("integer");

                    b.Property<int>("StageStatus")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("ASSESSMENT_RESULT");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AssessmentStage", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime?>("PullLastTime")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("PullState")
                        .HasColumnType("integer");

                    b.Property<int>("Stage")
                        .HasColumnType("integer");

                    b.Property<int?>("StageState")
                        .HasColumnType("integer");

                    b.Property<int?>("StageValue")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("ASSESSMENT_STAGE");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditActivity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("AUDIT_ACTIVITY");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditCycle", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int>("RatingPoint")
                        .HasColumnType("integer");

                    b.Property<string>("RatingPointName")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("AUDIT_CYCLE");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditFacility", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("ObjectClassId")
                        .HasColumnType("integer");

                    b.Property<string>("ObjectClassName")
                        .HasColumnType("text");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<string>("ParentName")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("AUDIT_FACILITY");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditProcess", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int>("ActivityId")
                        .HasColumnType("integer");

                    b.Property<string>("ActivityName")
                        .HasColumnType("text");

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<int>("FacilityId")
                        .HasColumnType("integer");

                    b.Property<string>("FacilityName")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("PersonCharge")
                        .HasColumnType("text");

                    b.Property<string>("PersonChargeEmail")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("AUDIT_PROCESS");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.BussinessActivity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("BUSSINESS_ACTIVITY");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.CatRiskLevel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasColumnType("text")
                        .HasColumnName("code");

                    b.Property<DateTime?>("CreateDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdate");

                    b.Property<DateTime?>("CreatedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("createdat");

                    b.Property<int?>("CreatedBy")
                        .HasColumnType("integer")
                        .HasColumnName("createdby");

                    b.Property<DateTime?>("DeletedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("deletedat");

                    b.Property<int?>("DeletedBy")
                        .HasColumnType("integer")
                        .HasColumnName("deletedby");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<bool?>("IsActive")
                        .HasColumnType("boolean")
                        .HasColumnName("is_active");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("ModifiedAt")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("modifiedat");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer")
                        .HasColumnName("modifiedby");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.Property<bool?>("Status")
                        .HasColumnType("boolean")
                        .HasColumnName("status");

                    b.HasKey("Id");

                    b.ToTable("CAT_RISK_LEVEL", t => t.ExcludeFromMigrations());
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.Formula", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("FORMULA");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RatingScale", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double?>("Max")
                        .HasColumnType("double precision");

                    b.Property<string>("MaxFunction")
                        .HasColumnType("text");

                    b.Property<double?>("Min")
                        .HasColumnType("double precision");

                    b.Property<string>("MinFunction")
                        .HasColumnType("text");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int>("RiskLevel")
                        .HasColumnType("integer");

                    b.Property<string>("RiskLevelName")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("RATING_SCALE");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskAssessmentScale", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<string>("Condition")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("LowerCondition")
                        .HasColumnType("integer");

                    b.Property<string>("LowerConditionName")
                        .HasColumnType("text");

                    b.Property<double?>("MaxValue")
                        .HasColumnType("double precision");

                    b.Property<double?>("MinValue")
                        .HasColumnType("double precision");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<double>("Point")
                        .HasColumnType("double precision");

                    b.Property<string>("RiskIssueCode")
                        .HasColumnType("text");

                    b.Property<int?>("RiskIssueCodeMethod")
                        .HasColumnType("integer");

                    b.Property<int?>("RiskIssueId")
                        .HasColumnType("integer");

                    b.Property<string>("RiskIssueName")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UpperCondition")
                        .HasColumnType("integer");

                    b.Property<string>("UpperConditionName")
                        .HasColumnType("text");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("RISK_ASSESSMENT_SCALE");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskIssue", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ApplyFor")
                        .HasColumnType("integer");

                    b.Property<string>("ApplyForName")
                        .HasColumnType("text");

                    b.Property<string>("ClassType")
                        .HasColumnType("text");

                    b.Property<string>("ClassTypeName")
                        .HasColumnType("text");

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<int?>("Formula")
                        .HasColumnType("integer");

                    b.Property<string>("FormulaName")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MethodId")
                        .HasColumnType("integer");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<int?>("ParentId")
                        .HasColumnType("integer");

                    b.Property<int?>("Proportion")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("RISK_ISSUE");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.ScoreBoard", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ApplyFor")
                        .HasColumnType("text");

                    b.Property<int>("AssessmentStageId")
                        .HasColumnType("integer");

                    b.Property<string>("AttachFile")
                        .HasColumnType("text");

                    b.Property<string>("AttachName")
                        .HasColumnType("text");

                    b.Property<string>("AuditCycleName")
                        .HasColumnType("text");

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("CurrentStatus")
                        .HasColumnType("integer");

                    b.Property<string>("Customer")
                        .HasColumnType("text");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<string>("ITSystem")
                        .HasColumnType("text");

                    b.Property<string>("InternalRegulations")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("LawRegulations")
                        .HasColumnType("text");

                    b.Property<string>("MainProcess")
                        .HasColumnType("text");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("ObjectCode")
                        .HasColumnType("text");

                    b.Property<int>("ObjectId")
                        .HasColumnType("integer");

                    b.Property<string>("ObjectName")
                        .HasColumnType("text");

                    b.Property<string>("Outsourcing")
                        .HasColumnType("text");

                    b.Property<double?>("Point")
                        .HasColumnType("double precision");

                    b.Property<string>("Project")
                        .HasColumnType("text");

                    b.Property<int?>("RatingScaleId")
                        .HasColumnType("integer");

                    b.Property<string>("RiskLevel")
                        .HasColumnType("text");

                    b.Property<int>("Stage")
                        .HasColumnType("integer");

                    b.Property<int?>("StageValue")
                        .HasColumnType("integer");

                    b.Property<string>("StateInfo")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<string>("Supplier")
                        .HasColumnType("text");

                    b.Property<string>("Target")
                        .HasColumnType("text");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("SCORE_BOARD");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.ScoreBoardIssue", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("ApplyFor")
                        .HasColumnType("text");

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<string>("Condition")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<string>("Formula")
                        .HasColumnType("text");

                    b.Property<int?>("FormulaId")
                        .HasColumnType("integer");

                    b.Property<int>("IssueId")
                        .HasColumnType("integer");

                    b.Property<int?>("IssueParentId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MethodId")
                        .HasColumnType("integer");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<double?>("Point")
                        .HasColumnType("double precision");

                    b.Property<string>("PointRange")
                        .HasColumnType("text");

                    b.Property<int?>("Proportion")
                        .HasColumnType("integer");

                    b.Property<int?>("ProportionModify")
                        .HasColumnType("integer");

                    b.Property<string>("Reason")
                        .HasColumnType("text");

                    b.Property<double?>("RiskValue")
                        .HasColumnType("double precision");

                    b.Property<int>("ScoreBoardId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("SCORE_BOARD_ISSUE");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.SystemCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(254)
                        .HasColumnType("character varying(254)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<bool>("Deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int>("DomainId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("ParentGroup")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("SYSTEM_CATEGORY");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.USER", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("department_id")
                        .HasColumnType("integer");

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<string>("full_name")
                        .HasColumnType("text");

                    b.Property<bool?>("is_active")
                        .HasColumnType("boolean");

                    b.Property<bool?>("is_deleted")
                        .HasColumnType("boolean");

                    b.HasKey("id");

                    b.ToTable("USERS", t => t.ExcludeFromMigrations());
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.UnitType", b =>
                {
                    b.Property<int>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("email")
                        .HasColumnType("text");

                    b.Property<bool?>("is_deleted")
                        .HasColumnType("boolean");

                    b.Property<string>("name")
                        .HasColumnType("text");

                    b.Property<bool?>("status")
                        .HasColumnType("boolean");

                    b.HasKey("id");

                    b.ToTable("UNIT_TYPE", t => t.ExcludeFromMigrations());
                });
#pragma warning restore 612, 618
        }
    }
}
