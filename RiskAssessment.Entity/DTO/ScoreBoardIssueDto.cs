
namespace RiskAssessment.Entity.DTO
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using RiskAssessment.Entity.DbEntities;
    using RiskAssessment.Entity.JsonOption;
    using RiskAssessment.Entity.ViewModel;

    public class ScoreBoardIssueDto : BaseDto<ScoreBoardIssue>
    {
        [JsonPropertyName("score_board")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int ScoreBoardId { get; set; }
        [JsonPropertyName("issue")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int IssueId { get; set; }
        [JsonPropertyName("issue_parent")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? IssueParentId { get; set; }

        //for risk
        [JsonPropertyName("point")]
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        public double? Point { get; set; }
        [JsonPropertyName("formula")]
        [JsonConverter(typeof(FormatString))]
        public string Formula { get; set; }
        [JsonPropertyName("point_range")]
        [JsonConverter(typeof(FormatString))]
        public string PointRange { get; set; }
        [JsonPropertyName("point_ranges")]
        public List<RiskPointRange> PointRanges
        {
            get
            {
                return string.IsNullOrEmpty(PointRange) ? null : JsonSerializer.Deserialize<List<RiskPointRange>>(PointRange);
            }
        }
        [JsonPropertyName("condition")]
        [JsonConverter(typeof(FormatString))]
        public string Condition { get; set; }
        [JsonPropertyName("risk_value")]
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        public double? RiskValue { get; set; }
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }
        [JsonPropertyName("method_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int MethodId { get; set; }
        //--includse description

        //for Proportion
        [JsonPropertyName("proportion")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Proportion { get; set; }
        [JsonPropertyName("proportion_modify")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ProportionModify { get; set; }
        [JsonPropertyName("reason")]
        [JsonConverter(typeof(FormatString))]
        public string Reason { get; set; }
        [JsonPropertyName("issues")]
        public List<ScoreBoardIssueDto> SubIssues { get; set; }
        [JsonPropertyName("parent")]
        public ScoreBoardIssueDto Parent { get; set; }
        [JsonPropertyName("has_child")]
        public bool HasChild { get; set; } = false;
        [JsonPropertyName("is_apply_post")]
        public int? IsApplyPost { get; set; }
        [JsonPropertyName("is_apply")]
        public bool? IsApply { get; set; }
    }
}
