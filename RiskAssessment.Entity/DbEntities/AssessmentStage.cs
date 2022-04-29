namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("ASSESSMENT_STAGE")]
    public class AssessmentStage : BaseEntity
    {
        public int Year { get; set; }
        public int Stage { get; set; }
        public int? StageValue { get; set; }
        public int? StageState{ get; set; }
        public int? PullState{ get; set; }
        public System.DateTime? PullLastTime{ get; set; }
    }
}
