using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class UpdateMindMapReportRequestDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "MembershipId phải > 0")]
        public int? MembershipId { get; set; }

        public DateTime? ReportDate { get; set; } // nullable, chỉ update khi có giá trị

        [MaxLength(4000, ErrorMessage = "ReportContent tối đa 4000 ký tự")]
        public string? ReportContent { get; set; }
    }
}
