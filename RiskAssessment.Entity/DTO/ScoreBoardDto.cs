namespace RiskAssessment.Entity.DTO
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    using RiskAssessment.Entity.DbEntities;
    using RiskAssessment.Entity.JsonOption;
    public class ScoreBoardDto : BaseDto<ScoreBoard>
    {
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("assessment_stage_id")]
        public int AssessmentStageId { get; set; }
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("object_id")]
        public int ObjectId { get; set; }
        [JsonPropertyName("apply_for")]
        [JsonConverter(typeof(FormatString))]
        public string ApplyFor { get; set; }

        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("current_status")]
        public int CurrentStatus { get; set; }

        //for assessment stage
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("year")]
        public int Year { get; set; }
        [JsonConverter(typeof(IntJsonConverter))]
        [JsonPropertyName("stage")]
        public int Stage { get; set; }
        [JsonConverter(typeof(IntNullableJsonConverter))]
        [JsonPropertyName("stage_value")]
        public int? StageValue { get; set; }


        //general info
        [JsonPropertyName("assessment_object")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectName { get; set; }
        [JsonPropertyName("object_code")]
        [JsonConverter(typeof(FormatString))]
        public string ObjectCode { get; set; }
        [JsonPropertyName("assessment_state")]
        [JsonConverter(typeof(FormatString))]
        public string StateInfo { get; set; }
        [JsonConverter(typeof(DoubleNullableJsonConverter))]
        [JsonPropertyName("assessment_point")]
        public double? Point { get; set; }
        [JsonPropertyName("assessment_risklevel")]
        [JsonConverter(typeof(FormatString))]
        public string RiskLevel { get; set; }

        //object info
        [JsonPropertyName("target_info")]
        [JsonConverter(typeof(FormatString))]
        public string Target { get; set; }
        [JsonPropertyName("mainprocess_info")]
        [JsonConverter(typeof(FormatString))]
        public string MainProcess { get; set; }
        [JsonPropertyName("itsystem_info")]
        [JsonConverter(typeof(FormatString))]
        public string ITSystem { get; set; }
        [JsonPropertyName("project_info")]
        [JsonConverter(typeof(FormatString))]
        public string Project { get; set; }
        [JsonPropertyName("outsourcing_info")]
        [JsonConverter(typeof(FormatString))]
        public string Outsourcing { get; set; }
        [JsonPropertyName("customer_info")]
        [JsonConverter(typeof(FormatString))]
        public string Customer { get; set; }
        [JsonPropertyName("supplier_info")]
        [JsonConverter(typeof(FormatString))]
        public string Supplier { get; set; }
        [JsonPropertyName("internal_info")]
        [JsonConverter(typeof(FormatString))]
        public string InternalRegulations { get; set; }
        [JsonPropertyName("law_info")]
        [JsonConverter(typeof(FormatString))]
        public string LawRegulations { get; set; }
        [JsonPropertyName("attach_path")]
        [JsonConverter(typeof(FormatString))]
        public string AttachFile { get; set; }
        [JsonPropertyName("attach_name")]
        [JsonConverter(typeof(FormatString))]
        public string AttachName { get; set; }
        [JsonPropertyName("rating_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? RatingScaleId { get; set; }

        public List<ScoreBoardFile> ScoreBoardFile { get; set; }
    }
}
