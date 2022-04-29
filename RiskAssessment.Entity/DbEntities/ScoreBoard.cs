namespace RiskAssessment.Entity.DbEntities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SCORE_BOARD")]
    public class ScoreBoard : BaseEntity
    {
        public ScoreBoard()
        {
            this.ScoreBoardFile = new HashSet<ScoreBoardFile>();
        }
        public int AssessmentStageId { get; set; }
        public string ObjectName { get; set; }
        public int ObjectId { get; set; }
        public string ApplyFor { get; set; }
        public string ObjectCode { get; set; }

        //for assessment stage
        public int Year { get; set; }
        public int Stage { get; set; }
        public int? StageValue { get; set; }
        public int CurrentStatus { get; set; }

        //general info
        public string StateInfo { get; set; }
        public double? Point { get; set; }
        public string RiskLevel { get; set; }
        public int? RatingScaleId { get; set; }
        public string AuditCycleName { get; set; }

        //object info
        public string Target { get; set; }
        public string MainProcess { get; set; }
        public string ITSystem { get; set; }
        public string Project { get; set; }
        public string Outsourcing { get; set; }
        public string Customer { get; set; }
        public string Supplier { get; set; }
        public string InternalRegulations { get; set; }
        public string LawRegulations { get; set; }
        public string AttachFile { get; set; }
        public string AttachName { get; set; }

        public virtual ICollection<ScoreBoardFile> ScoreBoardFile { get; set; }
    }
}
