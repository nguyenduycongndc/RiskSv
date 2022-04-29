using RiskAssessment.Entity.Enum;
using System.Text.Json.Serialization;

namespace RiskAssessment.Entity.CommonModel
{
    public class ModelSearch
    {
        public int? ExtendId { get; set; }
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
        [JsonPropertyName("user_create")]
        public int? UserCreate { get; set; }
        [JsonPropertyName("domain_id")]
        public int DomainId { get; set; }
        [JsonPropertyName("sorting")]
        public string SortBy { get; set; }
        [JsonPropertyName("sort_type")]
        public string SortType { get; set; }
    }
}
