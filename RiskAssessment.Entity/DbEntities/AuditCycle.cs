
using RiskAssessment.Entity.Enum;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AUDIT_CYCLE")]
    public class AuditCycle : BaseEntity
    {
        public int RatingPoint { get; set; }
        public string RatingPointName { get; set; }
    }
}
