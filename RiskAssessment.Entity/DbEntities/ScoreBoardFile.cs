using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RiskAssessment.Entity.DbEntities
{
    [Table("SCORE_BOARD_FILE")]
    public class ScoreBoardFile
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int ID { get; set; }
        [Column("scoreboard_id")]
        [JsonPropertyName("scoreboard_id")]
        public int ScoreBoardId { get; set; }
        [NotMapped]
        [ForeignKey("scoreboard_id")]
        public ScoreBoard ScoreBoard { get; set; }
        [Column("path")]
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [Column("file_type")]
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
        [Column("isdelete")]
        [JsonPropertyName("isdelete")]
        public bool? IsDelete { get; set; }

        [Column("create_at")]
        [JsonPropertyName("create_at")]
        public DateTime? CreateAt { get; set; }
        [Column("create_by")]
        [JsonPropertyName("create_by")]
        public int? CreatedBy { get; set; }

        [Column("delete_at")]
        [JsonPropertyName("delete_at")]
        public DateTime? DeleteAt { get; set; }
        [Column("delete_by")]
        [JsonPropertyName("delete_by")]
        public int? DeleteBy { get; set; }

    }
}
