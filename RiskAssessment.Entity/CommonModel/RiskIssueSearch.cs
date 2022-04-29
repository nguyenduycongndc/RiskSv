using RiskAssessment.Entity.JsonOption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiskAssessment.Entity.CommonModel
{
    public class RiskIssueSearch : ModelSearch
    {
        [JsonPropertyName("point")]
        [JsonConverter(typeof(DoubleJsonConverter))]
        public double Point { get; set; }
        [JsonPropertyName("method")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? MethodId { get; set; }
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ApplyFor { get; set; }
        [JsonPropertyName("class_type")]
        [JsonConverter(typeof(FormatString))]
        public string ClassType { get; set; }
    }
}
