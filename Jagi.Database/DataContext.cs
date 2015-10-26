using Jagi.Database.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Jagi.Database
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
            System.Data.Entity.Database.SetInitializer<DataContext>(null);
        }

        // virtual: for Unit Test NSubstitute mocking
        public virtual DbSet<CodeFile> CodeFiles { get; set; }
        public virtual DbSet<CodeDetail> CodeDetails { get; set; }
        public virtual DbSet<TableSchema> TableSchema { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TableSchema>().Map(m =>
            {
                m.MapInheritedProperties();
                m.ToTable("TableSchema");
            });
        }
    }
}