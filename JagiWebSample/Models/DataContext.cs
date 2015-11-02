using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace JagiWebSample.Models
{
    public class DataContext : DbContext
    {
        public DataContext(string connectionString) : base(connectionString) {}

        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<Patient>().Ignore(t => t.Name);
            modelBuilder.Entity<Patient>().Ignore(t => t.MobileNo);
            modelBuilder.Entity<Patient>().Ignore(t => t.Telno);
            modelBuilder.Entity<Patient>().Ignore(t => t.StreetNo);
            modelBuilder.Entity<Patient>().Ignore(t => t.IdCard);
            modelBuilder.Entity<Patient>().Property(t => t.BirthDate).HasColumnName("BIRTHDAY");

            base.OnModelCreating(modelBuilder);
        }
    }
}