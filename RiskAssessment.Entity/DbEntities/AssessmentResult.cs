namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System;
    [Table("ASSESSMENT_RESULT")]
    public class AssessmentResult : BaseEntity
    {
        public int ScoreBoardId { get; set; } = 0;
        public int StageStatus { get; set; } = 0;
        public int RiskLevel { get; set; } = 0;
        public int RiskLevelChange { get; set; } = 0;
        public string RiskLevelChangeName { get; set; }
        public bool Audit { get; set; } = false;
        public int? AuditReason { get; set; }
        public string PassAuditReason { get; set; }
        public DateTime? AuditDate { get; set; }
        public DateTime? LastAudit { get; set; }
        public string LastRiskLevel { get; set; }
        public int? AssessmentStatus { get; set; }
    }
}
