using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
using RiskAssessment.Entity.DbEntities;

namespace RiskAssessment.Entity.DTO
{
    public class AuditCycleDto : BaseDto<AuditCycle>
    {
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int RatingPoint { get; set; }
        [JsonPropertyName("risk_name")]
        [JsonConverter(typeof(FormatString))]
        public string RatingPointName { get; set; }
    }
}
