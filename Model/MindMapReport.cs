using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Model
{
    [Table("MindMapReport")]
    public class MindMapReport
    {
        [Key]
        public int ReportId { get; set; }

        [ForeignKey("MindMap")]
        public int MindMapId { get; set; }

        public int MembershipId { get; set; }

        public DateTime ReportDate { get; set; }

        public string? ReportContent { get; set; }

        // Navigation property
        public virtual MindMap? MindMap { get; set; }
    }
}
