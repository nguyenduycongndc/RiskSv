using System.Text.Json.Serialization;


namespace RiskAssessment.Entity.CommonModel
{
    public class AuditProcessSearch : ModelSearch
    {
        [JsonPropertyName("facility_id")]
        public int FacilityId { get; set; }
        [JsonPropertyName("activity_id")]
        public int ActivityId { get; set; }
    }
}
