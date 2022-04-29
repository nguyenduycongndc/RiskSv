using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskAssessment.Entity.DbEntities
{
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;
    [Table("APPROVAL_FUNCTION_STATUS")]
    public class ApproveFunction
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("item_id")]
        public int? ItemId { get; set; } // id chức năng đang xử lý
        [Column("function_name")]
        public string FunctionName { get; set; }
        [Column("function_code")]
        public string FunctionCode { get; set; } // mã của chức năng lấy theo menu
        [Column("approver")]
        public int? Approver { get; set; } // người duyệt
        [Column("approver_last")]
        public int? ApproverLast { get; set; } // người duyệt
        [Column("status_code")]
        public string StatusCode { get; set; }
        [Column("status_name")]
        public string StatusName { get; set; }
        [Column("reason")]
        public string Reason { get; set; } // lý do từ chối duyệt
        [Column("approval_date")]
        public DateTime? ApprovalDate { get; set; }
        [Column("path")]
        public string Path { get; set; }
        [Column("file_type")]
        public string FileType { get; set; }
    }
}
