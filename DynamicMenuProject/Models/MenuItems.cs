using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DynamicMenuProject.Models
{
    public partial class MenuItems
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentId { get; set; }
        public int MenuLevel { get; set; }
        public int MenuGrpId { get; set; }
    }
}
