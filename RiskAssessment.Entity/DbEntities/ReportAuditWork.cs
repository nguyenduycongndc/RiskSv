using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;

namespace RiskAssessment.Entity.DbEntities
{
    [Table("REPORT_AUDIT_WORK")]
    public class ReportAuditWork
    {
        public int id { get; set; }

        public int? status { get; set; }

        public int? auditwork_id { get; set; }

        public string auditwork_code { get; set; }

        public string auditwork_name { get; set; }

        public string year { get; set; }

        public DateTime? end_date_field { get; set; }

        public bool? is_deleted { get; set; }

        public int? audit_rating_level_total { get; set; }
    }
}
