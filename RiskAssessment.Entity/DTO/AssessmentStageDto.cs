using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class AssessmentStageDto : BaseDto<AssessmentStage>
    {
        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int Year { get; set; }
        [JsonPropertyName("stage")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int Stage { get; set; }
        [JsonPropertyName("value")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? StageValue { get; set; }
        [JsonPropertyName("state")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? StageState { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        [JsonPropertyName("pull_state")]
        public int? PullState { get; set; }
        [JsonPropertyName("pull_lasttime")]
        public System.DateTime? PullLastTime { get; set; }
    }
}
