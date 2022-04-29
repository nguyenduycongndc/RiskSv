namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    [Table("AUDIT_WORK_SCOPE")]
    public class AuditWorkScope
    {
        public int id { get; set; }

        public int? auditwork_id { get; set; }

        public int? auditprocess_id { get; set; }

        public int? year { get; set; }

        public string auditprocess_name { get; set; }

        public int? bussinessactivities_id { get; set; }

        public string bussinessactivities_name { get; set; }

        public int? auditfacilities_id { get; set; }

        public string auditfacilities_name { get; set; }

        public int? audit_rating_level_report { get; set; }

        public bool? is_deleted { get; set; }
    }
}
