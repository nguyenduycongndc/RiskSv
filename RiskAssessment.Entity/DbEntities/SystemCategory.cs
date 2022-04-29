using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("SYSTEM_CATEGORY")]
    public class SystemCategory : BaseEntity
    {
        public string ParentGroup { get; set; }
    }
}
