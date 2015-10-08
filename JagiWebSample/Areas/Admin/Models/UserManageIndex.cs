using JagiWebSample.Models;
using System.Collections.Generic;

namespace JagiWebSample.Areas.Admin.Models
{
    public class UserManageIndex
    {
        public string Search { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public string[] Roles { get; set; }
        public bool IsRolesEnabled { get; set; }
        public IEnumerable<ApplicationUser> OnlineUsers { get; set; }
    }
}