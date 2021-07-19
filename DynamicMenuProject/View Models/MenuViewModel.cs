using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicMenuProject.View_Models
{
    public class MenuViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentId { get; set; }
        public int MenuLevel { get; set; }
        public int MenuGrpId { get; set; }
    }
}
