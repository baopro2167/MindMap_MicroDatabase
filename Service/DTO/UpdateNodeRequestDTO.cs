using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class UpdateNodeRequestDTO
    {
        
        public int? ParentNodeId { get; set; }

        [MaxLength(2000, ErrorMessage = "Content tối đa 2000 ký tự")]
        public string? Content { get; set; }

        [EnumDataType(typeof(NodeType), ErrorMessage = "NodeType không hợp lệ")]
        public NodeType? NodeType { get; set; }

        [Range(-100000, 100000, ErrorMessage = "PositionX nằm ngoài khoảng cho phép")]
        public float? PositionX { get; set; }

        [Range(-100000, 100000, ErrorMessage = "PositionY nằm ngoài khoảng cho phép")]
        public float? PositionY { get; set; }

     
        public string? Color { get; set; }

        [RegularExpression(@"^(rectangle|circle|diamond)$",
            ErrorMessage = "Shape không hợp lệ (rectangle|circle|diamond)")]
        [MaxLength(50)]
        public string? Shape { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "OrderIndex phải >= 0")]
        public int? OrderIndex { get; set; }
    }
}
