using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class RiskAssessmentScaleDto : BaseDto<RiskAssessmentScale>
    {

        [JsonPropertyName("risk_issue")]
        public RiskIssueDto RiskIssue { get; set; }
        [JsonPropertyName("point")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Point { get; set; }
        [JsonPropertyName("condition")]
        [JsonConverter(typeof(FormatString))]
        public string Condition { get; set; }
        [JsonPropertyName("min_value")]
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        public double? MinValue { get; set; }
        [JsonPropertyName("max_value")]
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        public double? MaxValue { get; set; }
        [JsonPropertyName("min_condition")]
        public SystemCategoryDto MinCondition { get; set; }
        [JsonPropertyName("max_condition")]
        public SystemCategoryDto MaxCondition { get; set; }

        [JsonPropertyName("risk_issue_code")]
        [JsonConverter(typeof(FormatString))]
        public string IssueCode { get; set; }
        [JsonPropertyName("lower_condition")]
        [JsonConverter(typeof(FormatString))]
        public string LowerCondition { get; set; }
        [JsonPropertyName("upper_condition")]
        [JsonConverter(typeof(FormatString))]
        public string UpperCondition { get; set; }
        public int? Batch { get; set; }
        [JsonPropertyName("lower_condition_name")]
        [JsonConverter(typeof(FormatString))]
        public string LowerConditionName { get; set; }
        [JsonPropertyName("upper_condition_name")]
        [JsonConverter(typeof(FormatString))]
        public string UpperConditionName { get; set; }

    }
}
