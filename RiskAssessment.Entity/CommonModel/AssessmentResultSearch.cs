using RiskAssessment.Entity.JsonOption;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RiskAssessment.Entity.CommonModel
{
    public class AssessmentResultSearch: ScoreBoardSearch
    {
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }
        [JsonPropertyName("audit")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? audit { get; set; }
        [JsonPropertyName("board")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? BoardId { get; set; }
    }
    public class ScopeSearch : ScoreBoardSearch
    {
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? RiskLevel { get; set; }
        [JsonPropertyName("risk_level_name")]
        public string RiskLevelName { get; set; }
        [JsonPropertyName("keyprocess")]
        public string keyprocess { get; set; }
        [JsonPropertyName("keyfacility")]
        public string keyfacility { get; set; }
        [JsonPropertyName("keyactive")]
        public string keyactive { get; set; }
    }
    public class ScopeGetInfoItem { 
        [JsonPropertyName("select_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? select_id { get; set; }
        [JsonPropertyName("main_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? main_id { get; set; }

        [JsonPropertyName("apply_for")]
        public string apply_for { get; set; }
        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? year { get; set; }
    }
    public class ScopeGetInfo
    {
        [JsonPropertyName("list_asign")]
        
        public List<ScopeGetInfoItem> lstselect { get; set; }
        
    }
}
