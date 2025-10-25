using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
   public class AddMindMapRequestDTO
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "LessonId must be greater than 0.")]
        public int CreatedBy { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CreatedBy must be greater than 0.")]
        public int LessonId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsPublished { get; set; } = false;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
