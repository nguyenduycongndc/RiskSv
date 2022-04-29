using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class RatingScaleDto : BaseDto<RatingScale>
    {
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int RiskLevel { get; set; }
        [JsonPropertyName("risk_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevelName { get; set; }
        [JsonPropertyName("min")]
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        public double? Min { get; set; }
        [JsonPropertyName("max")]
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        public double? Max { get; set; }
        [JsonPropertyName("min_function")]
        [JsonConverter(typeof(FormatString))]
        public string MinFunction { get; set; }
        [JsonPropertyName("max_function")]
        [JsonConverter(typeof(FormatString))]
        public string MaxFunction { get; set; }

        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }
    }
}
