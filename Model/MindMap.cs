using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Model
{
    [Table("MindMap")]
    public class MindMap
    {
        [Key]
        public int MindMapId { get; set; }

        [ForeignKey("CreatedByUser")]
        public int CreatedByUser { get; set; }

        public int LessonId { get; set; }

        public string? Description { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();
        [JsonIgnore]
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();
        [JsonIgnore]
        public virtual ICollection<MindMapReport> Reports { get; set; } = new List<MindMapReport>();

    }
}
