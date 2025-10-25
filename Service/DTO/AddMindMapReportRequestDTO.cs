using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
   public class AddMindMapReportRequestDTO
    {
        [Required, Range(1, int.MaxValue, ErrorMessage = "MindMapId phải > 0")]
        public int MindMapId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "MembershipId phải > 0")]
        public int MembershipId { get; set; }

        [Required(ErrorMessage = "ReportDate bắt buộc")]
        public DateTime ReportDate { get; set; }  // nên truyền UTC

        [MaxLength(4000, ErrorMessage = "ReportContent tối đa 4000 ký tự")]
        public string? ReportContent { get; set; }
    }
}
