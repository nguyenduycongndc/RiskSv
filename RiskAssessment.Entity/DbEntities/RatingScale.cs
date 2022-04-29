using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("RATING_SCALE")]
    public class RatingScale : BaseEntity
    {
        public int RiskLevel { get; set; }
        public string RiskLevelName { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public string MinFunction { get; set; }
        public string MaxFunction { get; set; }
        public string ApplyFor { get; set; }
    }
}
