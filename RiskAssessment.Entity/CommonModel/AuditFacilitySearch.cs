using System.Text.Json.Serialization;

namespace RiskAssessment.Entity.CommonModel
{
    public class AuditFacilitySearch : ModelSearch
    {
        [JsonPropertyName("parent_id")]
        public int ParentId { get; set; }
        [JsonPropertyName("object_id")]
        public int ObjectId { get; set; }
    }
}
