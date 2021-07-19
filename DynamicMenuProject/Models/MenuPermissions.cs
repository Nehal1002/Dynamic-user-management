using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DynamicMenuProject.Models
{
    public partial class MenuPermissions
    {
        [Key]
        public Guid PermissionId { get; set; }
        public int MenuId { get; set; }
        public Guid RoleId { get; set; }
    }
}
