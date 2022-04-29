using System.Collections.Generic;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AUDIT_FACILITY")]
    public class AuditFacility : BaseEntity
    {
        public AuditFacility()
        {
            Childs = new List<AuditFacility>();
        }

        public int? ParentId { get; set; }
        public string ParentName { get; set; }
        public int? ObjectClassId { get; set; }
        public string ObjectClassName { get; set; }
        [NotMapped]
        public AuditFacility Parent { get; set; }

        [NotMapped]
        public List<AuditFacility> Childs { get; set; } = new List<AuditFacility>();
        [NotMapped]
        public string ParentCode { get; set; }
        [NotMapped]
        public int? Batch { get; set; }
        [NotMapped]
        public bool Valid { get; set; }
        [NotMapped]
        public string ancestor { get; set; }
    }
}
