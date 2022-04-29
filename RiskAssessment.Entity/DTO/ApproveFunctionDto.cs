using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using RiskAssessment.Entity.ViewModel;

namespace RiskAssessment.Entity.DTO
{
    public class ApproveFunctionDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("issue_parent")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ItemId { get; set; } // id chức năng đang xử lý

        [JsonPropertyName("function_name")]
        [JsonConverter(typeof(FormatString))]
        public string FunctionName { get; set; }

        [JsonPropertyName("function_code")]
        [JsonConverter(typeof(FormatString))]
        public string FunctionCode { get; set; } // mã của chức năng lấy theo menu

        [JsonPropertyName("approver")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Approver { get; set; } // người duyệt

        [JsonPropertyName("approver_last")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ApproverLast { get; set; } // người duyệt

        [JsonPropertyName("status_code")]
        [JsonConverter(typeof(FormatString))]
        public string StatusCode { get; set; }

        [JsonPropertyName("status_name")]
        [JsonConverter(typeof(FormatString))]
        public string StatusName { get; set; }

        [JsonPropertyName("reason")]
        [JsonConverter(typeof(FormatString))]
        public string Reason { get; set; } // lý do từ chối duyệt

        [JsonPropertyName("issue_parent")]
        public DateTime? ApprovalDate { get; set; }

        [JsonPropertyName("path")]
        [JsonConverter(typeof(FormatString))]
        public string Path { get; set; }

        [JsonPropertyName("file_type")]
        [JsonConverter(typeof(FormatString))]
        public string FileType { get; set; }

    }
}
