using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class AssessmentResultDto : BaseDto<AssessmentResult>
    {
        [JsonPropertyName("score_board")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int ScoreBoardId { get; set; } = 0;
        [JsonPropertyName("stage_status")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int StageStatus { get; set; } = 0;
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int RiskLevel { get; set; }
        [JsonPropertyName("audit")]
        public bool Audit { get; set; } = false;
        [JsonPropertyName("audit_reason")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? AuditReason { get; set; }
        [JsonPropertyName("pass_audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string PassAuditReason { get; set; }
        [JsonPropertyName("audit_date")]
        public DateTime? AuditDate { get; set; }
        [JsonPropertyName("last_audit")]
        public DateTime? LastAudit { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        [JsonPropertyName("last_risklevel")]
        public int? LastRiskLevel { get; set; }
    }
}
