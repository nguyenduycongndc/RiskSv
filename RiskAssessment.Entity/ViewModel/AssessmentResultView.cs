namespace RiskAssessment.Entity.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using RiskAssessment.Entity.DbEntities;
    using RiskAssessment.Entity.JsonOption;
    public class AssessmentResultView
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("code")]
        [JsonConverter(typeof(FormatString))]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        [JsonConverter(typeof(FormatString))]
        public string Name { get; set; }
        [JsonPropertyName("assessment_object")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectName { get; set; }
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("object_id")]
        public int ObjectId { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("object_code")]
        public string ObjectCode { get; set; }
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }

        [JsonPropertyName("apply_for_id")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyForId { get; set; }

        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        [JsonPropertyName("assessment_point")]
        public double? Point { get; set; }
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }
        [JsonPropertyName("risk_level_change")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int RiskLevelChange { get; set; } = 0;
        [JsonPropertyName("risk_level_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevelChangeName { get; set; }
        ///

        [JsonPropertyName("last_audit")]
        public string LastAudit { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("last_risklevel")]
        public string LastRiskLevel { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("audit_cycle")]
        public string AuditCycle { get; set; }
        [JsonPropertyName("audit")]
        public bool Audit { get; set; } = false;
        [JsonPropertyName("audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string AuditReason { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        [JsonPropertyName("audit_result_id")]
        public int? AuditReasonId { get; set; }
        [JsonPropertyName("pass_audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string PassAuditReason { get; set; }
        [JsonPropertyName("stage_status")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int StageStatus { get; set; } = 0;
        [JsonPropertyName("assessment_status")]
        public int? AssessmentStatus { get; set; } = 0;
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
    public class ScopeSearchResultView
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("facility_code")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityCode { get; set; }
        [JsonPropertyName("facility_name")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityName { get; set; }
        [JsonPropertyName("activity_code")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityCode { get; set; }
        [JsonPropertyName("activity_name")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityName { get; set; }
        [JsonPropertyName("process_code")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessCode { get; set; }
        [JsonPropertyName("process_name")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessName { get; set; }
        [JsonPropertyName("assessment_object")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectName { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("object_code")]
        public string ObjectCode { get; set; }
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("object_id")]
        public int ObjectId { get; set; }
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        [JsonPropertyName("assessment_point")]
        public double? Point { get; set; }
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }
        [JsonPropertyName("risk_level_change")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int RiskLevelChange { get; set; } = 0;
        [JsonPropertyName("risk_level_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevelChangeName { get; set; }
        [JsonPropertyName("is_level_change")]
        public bool? IsLevelChange { get; set; }
        [JsonPropertyName("last_audit")]
        public string LastAudit { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("last_risklevel")]
        public string LastRiskLevel { get; set; }
        [JsonConverter(typeof(FormatString))]
        [JsonPropertyName("audit_cycle")]
        public string AuditCycle { get; set; }
        [JsonPropertyName("audit")]
        public bool Audit { get; set; } = false;
        [JsonPropertyName("audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string AuditReason { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        [JsonPropertyName("audit_result_id")]
        public int? AuditReasonId { get; set; }
        [JsonPropertyName("stage_status")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int StageStatus { get; set; } = 0;
        [JsonPropertyName("assessment_status")]
        public int? AssessmentStatus { get; set; } = 0;
        [JsonPropertyName("sub_activities")]
        public List<ProcessResultView> sub_activities { get; set; }
    }
    public class ScopeResultView
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("process_id")]
        public int? ProcessID { get; set; }
        [JsonPropertyName("process_name")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessName { get; set; }
        [JsonPropertyName("facility_id")]
        public int? FacilityID { get; set; }
        [JsonPropertyName("facility_name")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityName { get; set; }
        [JsonPropertyName("activity_id")]
        public int? ActivityID { get; set; }
        [JsonPropertyName("activity_name")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityName { get; set; }
        [JsonPropertyName("audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string AuditReason { get; set; }
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }
        [JsonPropertyName("risk_level_change")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? RiskLevelChange { get; set; } = 0;
        [JsonPropertyName("risk_level_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevelChangeName { get; set; }
        [JsonPropertyName("is_level_change")]
        public bool? IsLevelChange { get; set; }
        [JsonPropertyName("is_show")]
        public bool? IsShow { get; set; }
        [JsonPropertyName("last_audit")]
        public string LastAudit { get; set; }
        [JsonPropertyName("last_audit_time")]
        public DateTime? LastAuditTime { get; set; }
        [JsonPropertyName("sub_activities")]
        public List<ScopeResultView> SubActivities { get; set; }
    }
    public class ScopeFacilityResultView
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("facility_id")]
        public int? FacilityID { get; set; }
        [JsonPropertyName("facility_name")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityName { get; set; }
        [JsonPropertyName("audit_reason")]
        [JsonConverter(typeof(FormatString))]
        public string AuditReason { get; set; }
        [JsonPropertyName("risk_level")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }
        [JsonPropertyName("risk_level_change")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? RiskLevelChange { get; set; } = 0;
        [JsonPropertyName("risk_level_name")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevelChangeName { get; set; }
        [JsonPropertyName("is_level_change")]
        public bool? IsLevelChange { get; set; }
        [JsonPropertyName("is_show")]
        public bool? IsShow { get; set; }
        [JsonPropertyName("last_audit")]
        public string LastAudit { get; set; }
        [JsonPropertyName("last_audit_time")]
        public DateTime? LastAuditTime { get; set; }
    }
    public class ScopeResultDetailView
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("facility_name")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityName { get; set; }
        [JsonPropertyName("activity_name")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityName { get; set; }
        [JsonPropertyName("process_code")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessCode { get; set; }
        [JsonPropertyName("process_name")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessName { get; set; }
    }
    public class ProcessResultView
    {
        [JsonPropertyName("id")]
        public int? ID { get; set; }
        [JsonPropertyName("facility_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? FacilityId { get; set; }
        [JsonPropertyName("facility_name")]
        [JsonConverter(typeof(FormatString))]
        public string FacilityName { get; set; }
        [JsonPropertyName("activity_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? ActivityId { get; set; }
        [JsonPropertyName("activity_name")]
        [JsonConverter(typeof(FormatString))]
        public string ActivityName { get; set; }

        [JsonPropertyName("process_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? ProcessId { get; set; }
        [JsonPropertyName("process_code")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessCode { get; set; }
        [JsonPropertyName("process_name")]
        [JsonConverter(typeof(FormatString))]
        public string ProcessName { get; set; }
    }

    public class RecentScoreBoard
    {
        [JsonPropertyName("assessment_risklevel")]
        [JsonConverter(typeof(FormatString))]
        public string assessment_risklevel { get; set; }
        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int Year { get; set; }
        [JsonPropertyName("stage")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int Stage { get; set; }
        [JsonPropertyName("stage_value")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? StageValue { get; set; }
    }

    public class RecentAudit
    {
        [JsonPropertyName("audit_rating_level_report")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int audit_rating_level_report { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(FormatString))]
        public string year { get; set; }

        [JsonPropertyName("audit_code")]
        [JsonConverter(typeof(FormatString))]
        public string audit_code { get; set; }

        [JsonPropertyName("time")]
        [JsonConverter(typeof(FormatString))]
        public string Time { get; set; }

        [JsonPropertyName("facility_id")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? facility_id { get; set; }
        [JsonPropertyName("auditid")]
        [JsonConverter(typeof(IntJsonConverter))]
        public int? auditid { get; set; }
    }
}
