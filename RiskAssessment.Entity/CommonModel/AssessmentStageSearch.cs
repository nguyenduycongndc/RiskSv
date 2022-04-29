using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;

namespace RiskAssessment.Entity.CommonModel
{
    public class AssessmentStageSearch : ModelSearch
    {
        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Year { get; set; }
        [JsonPropertyName("stage")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Stage { get; set; }
        [JsonPropertyName("value")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? StageValue { get; set; }
        [JsonPropertyName("state")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? StageState { get; set; }
    }
}
