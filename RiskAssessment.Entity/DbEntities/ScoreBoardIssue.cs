namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using System;

    [Table("SCORE_BOARD_ISSUE")]
    public class ScoreBoardIssue : BaseEntity
    {
        public int ScoreBoardId { get; set; }
        public int IssueId { get; set; }
        public int? IssueParentId { get; set; }
        public string ApplyFor { get; set; }

        //for risk
        public double? Point { get; set; }
        public string PointRange { get; set; }
        public string Condition { get; set; }
        public int? FormulaId { get; set; }
        public string Formula { get; set; }
        public double? RiskValue { get; set; }
        public int MethodId { get; set; }
        //--includse description

        //for Proportion
        public int? Proportion { get; set; }
        public int? ProportionModify { get; set; }
        public string Reason { get; set; }
        public DateTime? CreateDateIssue { get; set; }

        public bool? IsApply { get; set; }
    }
}
