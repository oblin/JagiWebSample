using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JagiWebSample.Areas.Admin.Models
{
    public class AdminDataContext : DbContext
    {
        public AdminDataContext() : base("DefaultConnection")
        {
            Database.SetInitializer<AdminDataContext>(null);
        }

        public DbSet<CodeFile> CodeFiles { get; set; }
        public DbSet<CodeDetail> CodeDetails { get; set; }
    }
}