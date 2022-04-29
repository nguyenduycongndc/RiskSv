namespace RiskAssessment.Entity.DTO
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using RiskAssessment.Entity.DbEntities;
    using RiskAssessment.Entity.JsonOption;
    using RiskAssessment.Entity.ViewModel;
    public class AuditWorkScopeDto
    {
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int id { get; set; }

        [JsonPropertyName("auditwork_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? auditwork_id { get; set; }

        [JsonPropertyName("auditprocess_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? auditprocess_id { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? year { get; set; }

        [JsonPropertyName("auditprocess_name")]
        [JsonConverter(typeof(FormatString))]
        public string auditprocess_name { get; set; }

        [JsonPropertyName("bussinessactivities_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? bussinessactivities_id { get; set; }

        [JsonPropertyName("bussinessactivities_name")]
        [JsonConverter(typeof(FormatString))]
        public string bussinessactivities_name { get; set; }

        [JsonPropertyName("auditfacilities_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? auditfacilities_id { get; set; }

        [JsonPropertyName("auditfacilities_name")]
        [JsonConverter(typeof(FormatString))]
        public string auditfacilities_name { get; set; }

        [JsonPropertyName("audit_rating_level_report")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? audit_rating_level_report { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }
    }
}
