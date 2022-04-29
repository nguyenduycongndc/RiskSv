namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.ComponentModel.DataAnnotations;
    using System;
    public class USER
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        public string full_name { get; set; }
        public string email { get; set; }
        public bool? is_active { get; set; }
        public bool? is_deleted { get; set; }
        public int? department_id { get; set; }
    }
}
