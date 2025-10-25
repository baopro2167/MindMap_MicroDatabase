using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
    public class NodeReadDTO
    {
        public int NodeId { get; set; }
        public int MindMapId { get; set; }
        public int? ParentNodeId { get; set; }
        public string? Content { get; set; }
        public Model.Enums.NodeType NodeType { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public string? Color { get; set; }
        public string? Shape { get; set; }
        public int OrderIndex { get; set; }
    }
}
