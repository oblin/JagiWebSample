using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public DbSet<TableSchema> TableSchema { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableSchema>()
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<TableSchema>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("TableSchema");
            });
        }
    }
}