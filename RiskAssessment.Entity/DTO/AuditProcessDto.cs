using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class AuditProcessDto : BaseDto<AuditProcess>
    {
        [JsonPropertyName("facility_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int FacilityId { get; set; }
        [JsonPropertyName("facility_name")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityName { get; set; }
        [JsonPropertyName("activity_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int ActivityId { get; set; }
        [JsonPropertyName("activity_name")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityName { get; set; }
        [JsonPropertyName("person_charge")]
        [JsonConverter(typeof(FormatString))]
        public string PersonCharge { get; set; }
        [JsonPropertyName("person_charge_email")]
        [JsonConverter(typeof(FormatString))]
        public string PersonChargeEmail { get; set; }
        [JsonPropertyName("facility_code")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityCode { get; set; }
        [JsonPropertyName("activity_code")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityCode { get; set; }

        [JsonPropertyName("risk_type_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskTypeName { get; set; }

    }
}
