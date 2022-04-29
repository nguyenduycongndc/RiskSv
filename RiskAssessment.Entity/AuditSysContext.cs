using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace RiskAssessment.Entity.DbEntities
{
    public class AuditSysContext : DbContext
    {
        public AuditSysContext()
        {
        }
        public AuditSysContext(DbContextOptions<AuditSysContext> options) : base(options)
        {
        }

        public DbSet<AuditActivity> AuditActivities { get; set; }
        public DbSet<AuditCycle> AuditCycles { get; set; }
        public DbSet<AuditFacility> AuditFacilities { get; set; }
        public DbSet<AuditProcess> AuditProcess { get; set; }
        public DbSet<BussinessActivity> BussinessActivities { get; set; }
        public DbSet<RatingScale> RatingScales { get; set; }
        public DbSet<RiskAssessmentScale> RiskAssessmentScales { get; set; }
        public DbSet<RiskIssue> RiskIssues { get; set; }
        public DbSet<SystemCategory> SystemCategories { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<CatRiskLevel> CatRiskLevels { get; set; }
        public DbSet<AssessmentStage> AssessmentStages { get; set; }
        public DbSet<ScoreBoard> ScoreBoards { get; set; }
        public DbSet<ScoreBoardIssue> ScoreBoardIssues { get; set; }
        public DbSet<AssessmentResult> AssessmentResults { get; set; }
        public DbSet<USER> Users { get; set; }
        public DbSet<UnitType> UnitTypes { get; set; }
        public DbSet<AuditWorkScope> AuditWorkScopes { get; set; }
        public DbSet<ReportAuditWork> ReportAuditWorks { get; set; }
        public DbSet<AuditWork> AuditWorks { get; set; }
        public DbSet<ApproveFunction> ApproveFunction { get; set; }
        public DbSet<ScoreBoardFile> ScoreBoardFile { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditActivity>().ToTable("AUDIT_ACTIVITY");
            modelBuilder.Entity<AuditCycle>().ToTable("AUDIT_CYCLE");
            modelBuilder.Entity<AuditFacility>().ToTable("AUDIT_FACILITY");
            modelBuilder.Entity<AuditProcess>().ToTable("AUDIT_PROCESS");
            modelBuilder.Entity<BussinessActivity>().ToTable("BUSSINESS_ACTIVITY");
            modelBuilder.Entity<RatingScale>().ToTable("RATING_SCALE");
            modelBuilder.Entity<RiskAssessmentScale>().ToTable("RISK_ASSESSMENT_SCALE");
            modelBuilder.Entity<RiskIssue>().ToTable("RISK_ISSUE");
            modelBuilder.Entity<SystemCategory>().ToTable("SYSTEM_CATEGORY");
            modelBuilder.Entity<Formula>().ToTable("FORMULA");
            modelBuilder.Entity<AssessmentStage>().ToTable("ASSESSMENT_STAGE");
            modelBuilder.Entity<ScoreBoard>().ToTable("SCORE_BOARD");
            modelBuilder.Entity<ScoreBoardIssue>().ToTable("SCORE_BOARD_ISSUE");
            modelBuilder.Entity<AssessmentResult>().ToTable("ASSESSMENT_RESULT");
            modelBuilder.Entity<AuditWorkScope>().ToTable("AUDIT_WORK_SCOPE");
            modelBuilder.Entity<ReportAuditWork>().ToTable("REPORT_AUDIT_WORK");
            modelBuilder.Entity<AuditWork>().ToTable("AUDIT_WORK");
            modelBuilder.Entity<ApproveFunction>().ToTable("APPROVAL_FUNCTION_STATUS");
            modelBuilder.Entity<ScoreBoardFile>().ToTable("SCORE_BOARD_FILE");

            //from other resource
            modelBuilder.Entity<CatRiskLevel>().ToTable("CAT_RISK_LEVEL")
                .Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<USER>().ToTable("USERS")
                .Metadata.SetIsTableExcludedFromMigrations(true);
            modelBuilder.Entity<UnitType>().ToTable("UNIT_TYPE")
                .Metadata.SetIsTableExcludedFromMigrations(true);

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionBuilder)
        {
            //optionBuilder.UseNpgsql("Host=192.168.50.151;Port=5432;Username=kitano_dbo;Password=dbo#123;Database=kitano;");
        }

    }
}
