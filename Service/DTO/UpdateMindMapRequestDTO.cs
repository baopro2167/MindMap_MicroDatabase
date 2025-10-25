using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.DTO
{
   public class UpdateMindMapRequestDTO
    {
       
        public string? Description { get; set; }

     
        public bool IsPublished { get; set; }

  
        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
