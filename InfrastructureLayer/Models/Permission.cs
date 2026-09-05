using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g. "ManageOrders"
        public string? Description { get; set; }

        public ICollection<EmployeePermission> EmployeePermissions { get; set; } = new List<EmployeePermission>();
    }
}
