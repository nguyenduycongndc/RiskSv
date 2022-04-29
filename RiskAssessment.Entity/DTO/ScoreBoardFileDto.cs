using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiskAssessment.Entity.DTO
{
    public class ScoreBoardFileDto
    {
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [JsonPropertyName("scoreboard_id")]
        public int ScoreBoardId { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
        [JsonPropertyName("isdelete")]
        public bool? IsDelete { get; set; }

        [JsonPropertyName("create_at")]
        public DateTime? CreateAt { get; set; }
        [JsonPropertyName("create_by")]
        public int? CreatedBy { get; set; }

        [JsonPropertyName("delete_at")]
        public DateTime? DeleteAt { get; set; }
        [JsonPropertyName("delete_by")]
        public int? DeleteBy { get; set; }

    }
}
