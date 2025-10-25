﻿using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    [Table("Node")]
    public class Node
    {
        [Key]
        public int NodeId { get; set; }

        [ForeignKey("MindMap")]
        public int MindMapId { get; set; }

        public int? ParentNodeId { get; set; }

        public string? Content { get; set; }

        public NodeType NodeType { get; set; } // ✅ dùng enum tách riêng

        public float PositionX { get; set; }

        public float PositionY { get; set; }

        public string? Color { get; set; }

        public string? Shape { get; set; }

        public int OrderIndex { get; set; }

        public virtual MindMap? MindMap { get; set; }
        public virtual Node? ParentNode { get; set; }
        public virtual ICollection<Node> ChildNodes { get; set; } = new List<Node>();

        [InverseProperty("SourceNode")]
        public virtual ICollection<Branch> OutgoingBranches { get; set; } = new List<Branch>();

        [InverseProperty("TargetNode")]
        public virtual ICollection<Branch> IncomingBranches { get; set; } = new List<Branch>();
    }
}
