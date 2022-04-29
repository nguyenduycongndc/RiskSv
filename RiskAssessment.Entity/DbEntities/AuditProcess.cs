namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("AUDIT_PROCESS")]
    public class AuditProcess : BaseEntity
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public int ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string PersonCharge { get; set; }
        public string PersonChargeEmail { get; set; }
    }
}
