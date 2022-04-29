using RiskAssessment.Entity.Enum;
using System.Collections.Generic;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RISK_ISSUE")]
    public class RiskIssue : BaseEntity
    {
        public int? ParentId { get; set; }
        public int? ApplyFor { get; set; }
        public string ApplyForName { get; set; }
        public string ClassType { get; set; }
        public string ClassTypeName { get; set; }
        public int? Formula { get; set; }
        public string FormulaName { get; set; }
        public int? Proportion { get; set; }
        public int MethodId { get; set; }
    }
}
