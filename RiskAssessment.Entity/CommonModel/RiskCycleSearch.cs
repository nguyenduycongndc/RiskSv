using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;

namespace RiskAssessment.Entity.CommonModel
{
    public class RiskCycleSearch : ModelSearch
    {
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? RiskLevel { get; set; }
    }
}
