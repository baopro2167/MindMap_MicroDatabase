using Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class BranchReadDTO
    {
        public int BranchId { get; set; }
        public int MindMapId { get; set; }
        public int SourceNodeId { get; set; }
        public int TargetNodeId { get; set; }
        public BranchType BranchType { get; set; }
        public string? Label { get; set; }
        public string? Style { get; set; }

        // Tuỳ nhu cầu: kèm node “slim” để client đủ render
        public NodeReadDTO? SourceNode { get; set; }
        public NodeReadDTO? TargetNode { get; set; }
    }
}
