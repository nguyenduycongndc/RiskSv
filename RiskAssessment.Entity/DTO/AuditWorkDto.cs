using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using RiskAssessment.Entity.ViewModel;

namespace RiskAssessment.Entity.DTO
{
    public class AuditWorkDto
    {
        //ExecutionStatus : Trạng thái thực hiện 1 chưa thực hiện , 2 đang thực hiện , 3 hoàn thành
        [JsonPropertyName("id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int ID { get; set; }
        [JsonPropertyName("code")]
        [JsonConverter(typeof(FormatString))]
        public string code { get; set; }
        [JsonPropertyName("name")]
        [JsonConverter(typeof(FormatString))]
        public string name { get; set; }
        [JsonPropertyName("status")]
        [JsonConverter(typeof(StatusImport))]
        public bool status { get; set; }

        [JsonPropertyName("execution_status")]
        [JsonConverter(typeof(IntNullableJsonConverter))]

        public int? execution_status { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? is_deleted { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(FormatString))]

        public string year { get; set; }
    }
}
