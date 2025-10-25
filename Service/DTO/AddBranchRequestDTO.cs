using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class AddBranchRequestDTO
    {
        [Required, Range(1, int.MaxValue)]
        public int MindMapId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int SourceNodeId { get; set; }

        [Required, Range(1, int.MaxValue)]
        public int TargetNodeId { get; set; }

        [Required]
        [EnumDataType(typeof(BranchType), ErrorMessage = "BranchType không hợp lệ")]
        public BranchType BranchType { get; set; }

        [MaxLength(500)]
        public string? Label { get; set; }

        [MaxLength(100)]
        public string? Style { get; set; } // ví dụ "solid|dashed|dotted" tuỳ bạn
    }
}
