using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using RiskAssessment.Entity.ViewModel;
using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class RiskIssueDto : BaseDto<RiskIssue>
    {
        public RiskIssueDto()
        {
            SubIssues = new List<RiskIssueDto>();
        }
        [JsonPropertyName("parent")]
        public RiskIssueDto Parent { get; set; }

        [JsonPropertyName("apply_for")]
        public SystemCategoryDto ApplyFor { get; set; }

        //[JsonPropertyName("class_type")]
        //public UnitTypeView ClassType { get; set; }

        [JsonPropertyName("class_type")]
        [JsonConverter(typeof(FormatString))]
        public string ClassType { get; set; }
        [JsonPropertyName("class_type_name")]
        [JsonConverter(typeof(FormatString))]
        public string ClassTypeName { get; set; }

        [JsonPropertyName("formula")]
        public FormulaDto Formula { get; set; }
        [JsonPropertyName("proportion")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Proportion { get; set; }
        [JsonPropertyName("method_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int MethodId { get; set; }
        [JsonPropertyName("issues")]
        public List<RiskIssueDto> SubIssues { get; set; }




        [JsonPropertyName("applyfor_code")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyCode { get; set; }

        [JsonPropertyName("class_code")]
        [JsonConverter(typeof(FormatString))]
        public string ClassCode { get; set; }

        [JsonPropertyName("formula_code")]
        [JsonConverter(typeof(FormatString))]
        public string FormulaCode { get; set; }

        [JsonPropertyName("parent_code")]
        [JsonConverter(typeof(FormatString))]
        public string ParentCode { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Batch { get; set; }

        public string ancestor { get; set; }
        [JsonPropertyName("parent_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ParentId { get; set; }
    }
}
