using RiskAssessment.Entity.DbEntities;
using RiskAssessment.Entity.JsonOption;
using System.Text.Json.Serialization;
namespace RiskAssessment.Entity.DTO
{
    public class SystemCategoryDto : BaseDto<SystemCategory>
    {
        [JsonPropertyName("parent_group")]
        [JsonConverter(typeof(FormatString))]
        public string ParentGroup { get; set; }
    }
}
