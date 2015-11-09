using Jagi.Interface;
using Jagi.Mvc;
using JagiWebSample.Utility;
using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace JagiWebSample.Models
{
    public class DataContext : DbContext
    {
        private readonly string _not_login_user = "NotLoginUser";
        private CurrentRequest _currentRequest;

        public DataContext(string connectionString) : base(connectionString) { }

        public DataContext(string connectionString, CurrentRequest current)
            : base(connectionString)
        {
            _currentRequest = current;
        }

        public DbSet<Patient> Patients { get; set; }
        public DbSet<DataAccessLog> AccessLogs { get; set; }

        public DbSet Set(string name)
        {
            var nameSpace = this.GetType().Namespace;
            Type type = Type.GetType(nameSpace + "." + name);
            return base.Set(type);
        }

        /// <summary>
        /// 在原始 SaveChange() 之上，加入自動處理 Entity 的 Create Date, User...
        /// 並且自動加入 DataAccessLog table 內
        /// </summary>
        /// <returns></returns>
        public int Save()
        {
            var trackables = ChangeTracker.Entries<Entity>();

            if (trackables != null)
            {
                if (!_currentRequest.IsAuthenticated)
                    throw new UnauthorizedAccessException(ConstantString.UNAUTHORIZED_ACCESS_EXCEPTION);

                string currentUser = _currentRequest.User == null
                        ? _not_login_user : _currentRequest.User.UserName;

                DataAccessLog log = new DataAccessLog();

                // Process Added
                foreach (var item in trackables.Where(t => t.State == EntityState.Added))
                {
                    item.Entity.CreatedDate = System.DateTime.Now;
                    item.Entity.CreatedUser = currentUser;

                    log.SetEntityDescription(item, ActionType.Create);
                }
                // Process Modified
                foreach (var item in trackables.Where(t => t.State == EntityState.Modified))
                {
                    item.Entity.ModifiedDate = System.DateTime.Now;
                    item.Entity.ModifiedUser = currentUser;
                    log.SetEntityDescription(item, ActionType.Create);
                }
                // Process Deleted
                foreach (var item in trackables.Where(t => t.State == EntityState.Deleted))
                {
                    item.Entity.ModifiedDate = System.DateTime.Now;
                    item.Entity.ModifiedUser = currentUser;
                    log.SetEntityDescription(item, ActionType.Create);
                }

                if (!string.IsNullOrEmpty(log.ActionDescription))
                {
                    log.UserName = currentUser;
                    log.AccessDate = DateTime.Now;
                    this.AccessLogs.Add(log);   
                }
            }

            return base.SaveChanges();
        }

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