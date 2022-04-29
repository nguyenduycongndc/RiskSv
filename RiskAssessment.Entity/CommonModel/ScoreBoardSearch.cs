using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.CommonModel
{
    public class ScoreBoardSearch : AssessmentStageSearch
    {
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }
        [JsonPropertyName("object_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ObjectId { get; set; }
        [JsonPropertyName("object_name")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectName { get; set; }
    }
}
