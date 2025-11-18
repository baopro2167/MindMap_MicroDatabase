using Model.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Model
{
    [Table("Branch")]
    public class Branch
    {
        [Key]
        public int BranchId { get; set; }

        [ForeignKey("MindMap")]
        public int MindMapId { get; set; }

        [ForeignKey("SourceNode")]
        public int SourceNodeId { get; set; }

        [ForeignKey("TargetNode")]
        public int TargetNodeId { get; set; }

        public BranchType BranchType { get; set; } // ✅ enum riêng

        public string? Label { get; set; }

        public string? Style { get; set; }
        [JsonIgnore]
        public virtual MindMap? MindMap { get; set; }
        [JsonIgnore]
        public virtual Node? SourceNode { get; set; }
        [JsonIgnore]
        public virtual Node? TargetNode { get; set; }
     
    }
}
