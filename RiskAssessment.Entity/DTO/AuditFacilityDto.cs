using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class AuditFacilityDto : BaseDto<AuditFacility>
    {
        [JsonPropertyName("parent_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ParentId { get; set; }
        [JsonPropertyName("parent")]
        public AuditFacilityDto Parent { get; set; }

        [JsonPropertyName("parent_name")]
        [JsonConverter(typeof(FormatString))]
        public string ParentName { get; set; }

        [JsonPropertyName("object_class_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ObjectClassId { get; set; }
        [JsonPropertyName("object_class_name")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectClassName { get; set; }

        public List<AuditFacilityDto> Childs { get; set; } = new List<AuditFacilityDto>();


        [JsonPropertyName("class_code")]
        [JsonConverter(typeof(FormatString))]
        public string ClassCode { get; set; }

        [JsonPropertyName("parent_code")]
        [JsonConverter(typeof(FormatString))]
        public string ParentCode { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Batch { get; set; }

        [JsonPropertyName("ancestor")]
        public string ancestor { get; set; }
    }
}
