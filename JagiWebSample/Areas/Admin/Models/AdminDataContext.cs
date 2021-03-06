using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

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
        public DbSet<Address> Address { get; set; }

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

            modelBuilder.Entity<Address>().Map(m =>
            {
                m.ToTable("Address");
            });
        }
    }
}