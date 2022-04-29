using System.Collections.Generic;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("BUSSINESS_ACTIVITY")]
    public class BussinessActivity : BaseEntity
    {
        public int? ParentId { get; set; }

        public BussinessActivity()
        {
            SubActivities = new List<BussinessActivity>();
        }
        [NotMapped]
        public BussinessActivity Parent { get; set; }
        [NotMapped]
        public List<BussinessActivity> SubActivities { get; set; }

        [NotMapped]
        public string ParentCode { get; set; }
        [NotMapped]
        public int? Batch { get; set; }
        [NotMapped]
        public string ancestor { get; set; }

        [NotMapped]
        public bool Valid { get; set; }
    }
}
