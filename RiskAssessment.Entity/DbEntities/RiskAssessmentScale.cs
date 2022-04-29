using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RISK_ASSESSMENT_SCALE")]
    public class RiskAssessmentScale : BaseEntity
    {
        public int? RiskIssueId { get; set; }
        public string RiskIssueName { get; set; }
        public string RiskIssueCode { get; set; }
        public int? RiskIssueCodeMethod { get; set; }
        public double Point { get; set; }
        public string Condition { get; set; }
        public double? MinValue { get; set; }
        public double? MaxValue { get; set; }
        public int? LowerCondition { get; set; }
        public string LowerConditionName { get; set; }
        public int? UpperCondition { get; set; }
        public string UpperConditionName { get; set; }
    }
}
