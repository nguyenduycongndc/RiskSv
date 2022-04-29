using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RiskAssessment.Entity.DTO
{
    public class BussinessActivityDto : BaseDto<BussinessActivity>
    {
        public BussinessActivityDto()
        {
            SubActivities = new List<BussinessActivityDto>();
        }
        [JsonPropertyName("parent_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ParentId { get; set; }
        [JsonPropertyName("parent")]
        public BussinessActivityDto Parent { get; set; }
        [JsonPropertyName("sub_activities")]
        public List<BussinessActivityDto> SubActivities { get; set; }


        [JsonPropertyName("parent_code")]
        [JsonConverter(typeof(FormatString))]
        public string ParentCode { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Batch { get; set; }
        [JsonPropertyName("ancestor")]
        public string ancestor { get; set; }
    }
}
