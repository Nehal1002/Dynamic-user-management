using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicMenuProject.View_Models
{
    public class PermissionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int ParentId { get; set; }
        public int MenuLevel { get; set; }
        public bool HasAccess { get; set; }
        public int MenuGrpId { get; set; }
    }
}
