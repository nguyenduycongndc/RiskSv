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
    [Migration("20210919091258_update db relationship")]
    partial class updatedbrelationship
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditActivity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("AuditActivity");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditCycle", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("RatingPoint")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("AuditCycle");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditFacility", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("ObjectClassID")
                        .HasColumnType("integer");

                    b.Property<int?>("ParentID")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("ObjectClassID");

                    b.HasIndex("ParentID");

                    b.ToTable("AuditFacility");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditProcess", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ActivityID")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int?>("FacilityID")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("PersonCharge")
                        .HasColumnType("text");

                    b.Property<string>("PersonChargeEmail")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("ActivityID");

                    b.HasIndex("FacilityID");

                    b.ToTable("AuditProcess");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.BussinessActivity", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("BussinessActivity");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.Formula", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("Formula");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RatingScale", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<double?>("Max")
                        .HasColumnType("double precision");

                    b.Property<double?>("Min")
                        .HasColumnType("double precision");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("RatingScale");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskAssessmentScale", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<string>("Condition")
                        .HasColumnType("text");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("MaxCondition")
                        .HasColumnType("integer");

                    b.Property<double?>("MaxValue")
                        .HasColumnType("double precision");

                    b.Property<int?>("MinCondition")
                        .HasColumnType("integer");

                    b.Property<double?>("MinValue")
                        .HasColumnType("double precision");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<double>("Point")
                        .HasColumnType("double precision");

                    b.Property<int?>("RiskIssueID")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("RiskIssueID");

                    b.ToTable("RiskAssessmentScale");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskIssue", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("ApplyForID")
                        .HasColumnType("integer");

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<int?>("FormulaID")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("MethodId")
                        .HasColumnType("integer");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int?>("Proportion")
                        .HasColumnType("integer");

                    b.Property<int?>("RiskIssueID")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.HasIndex("ApplyForID");

                    b.HasIndex("FormulaID");

                    b.HasIndex("RiskIssueID");

                    b.ToTable("RiskIssue");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.SystemCategory", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<string>("Code")
                        .HasMaxLength(500)
                        .HasColumnType("character varying(500)");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("timestamp without time zone");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<DateTime?>("LastModified")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int?>("ModifiedBy")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("ParentGroup")
                        .HasColumnType("text");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserCreate")
                        .HasColumnType("integer");

                    b.HasKey("ID");

                    b.ToTable("SystemCategory");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditFacility", b =>
                {
                    b.HasOne("RiskAssessment.Entity.DbEntities.SystemCategory", "ObjectClass")
                        .WithMany()
                        .HasForeignKey("ObjectClassID");

                    b.HasOne("RiskAssessment.Entity.DbEntities.AuditFacility", "Parent")
                        .WithMany("SubFacilities")
                        .HasForeignKey("ParentID");

                    b.Navigation("ObjectClass");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditProcess", b =>
                {
                    b.HasOne("RiskAssessment.Entity.DbEntities.AuditActivity", "Activity")
                        .WithMany()
                        .HasForeignKey("ActivityID");

                    b.HasOne("RiskAssessment.Entity.DbEntities.AuditFacility", "Facility")
                        .WithMany()
                        .HasForeignKey("FacilityID");

                    b.Navigation("Activity");

                    b.Navigation("Facility");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskAssessmentScale", b =>
                {
                    b.HasOne("RiskAssessment.Entity.DbEntities.RiskIssue", "RiskIssue")
                        .WithMany()
                        .HasForeignKey("RiskIssueID");

                    b.Navigation("RiskIssue");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskIssue", b =>
                {
                    b.HasOne("RiskAssessment.Entity.DbEntities.SystemCategory", "ApplyFor")
                        .WithMany()
                        .HasForeignKey("ApplyForID");

                    b.HasOne("RiskAssessment.Entity.DbEntities.Formula", "Formula")
                        .WithMany()
                        .HasForeignKey("FormulaID");

                    b.HasOne("RiskAssessment.Entity.DbEntities.RiskIssue", null)
                        .WithMany("SubIssues")
                        .HasForeignKey("RiskIssueID");

                    b.Navigation("ApplyFor");

                    b.Navigation("Formula");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.AuditFacility", b =>
                {
                    b.Navigation("SubFacilities");
                });

            modelBuilder.Entity("RiskAssessment.Entity.DbEntities.RiskIssue", b =>
                {
                    b.Navigation("SubIssues");
                });
#pragma warning restore 612, 618
        }
    }
}
