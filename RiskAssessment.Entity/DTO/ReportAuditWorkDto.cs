using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using RiskAssessment.Entity.ViewModel;

namespace RiskAssessment.Entity.DTO
{
    public class ReportAuditWorkDto
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int id { get; set; }

        [JsonPropertyName("status")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? status { get; set; }

        [JsonPropertyName("auditwork_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? auditwork_id { get; set; }

        [JsonPropertyName("auditwork_code")]
        [JsonConverter(typeof(FormatString))]
        public string auditwork_code { get; set; }

        [JsonPropertyName("auditwork_name")]
        [JsonConverter(typeof(FormatString))]
        public string auditwork_name { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(FormatString))]
        public string year { get; set; }

        [JsonPropertyName("end_date_field")]
        public DateTime? end_date_field { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }

        [JsonPropertyName("audit_rating_level_total")]
        public int? audit_rating_level_total { get; set; }


    }
}
