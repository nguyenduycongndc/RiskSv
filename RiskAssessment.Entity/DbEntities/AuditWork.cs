using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System;
using System.ComponentModel.DataAnnotations;

namespace RiskAssessment.Entity.DbEntities
{
    public class AuditWork
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [MaxLength(254)]
        public string Code { get; set; }
        [MaxLength(500)]
        public string Name { get; set; }
        public int? Status { get; set; }
        //ExecutionStatus : Trạng thái thực hiện 1 chưa thực hiện , 2 đang thực hiện , 3 hoàn thành
        public int? ExecutionStatus { get; set; }

        public bool? is_deleted { get; set; }

        public string Year { get; set; }
    }
}
