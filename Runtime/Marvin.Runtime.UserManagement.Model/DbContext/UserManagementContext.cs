using System.Data.Common;
using System.Data.Entity;
using Marvin.Model;
using Marvin.Model.Npgsql;

namespace Marvin.Runtime.UserManagement.Model
{
    /// <summary>
    /// The DBContext of this database model.
    /// </summary>
    [DbConfigurationType(typeof(NpgsqlConfiguration))]
    public class UserManagementContext : MarvinDbContext
    {
        public UserManagementContext()
        {
            
        }

        public UserManagementContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        public UserManagementContext(DbConnection dbConnection, ContextMode mode) : base(dbConnection, mode)
        {
        }

        public virtual DbSet<Library> Libraries { get; set; }
    
        public virtual DbSet<UserGroup> UserGroups { get; set; }
    
        public virtual DbSet<Application> Applications { get; set; }

        public virtual DbSet<LibraryAccess> LibraryAccesses { get; set; }

        public virtual DbSet<ApplicationAccess> ApplicationAccesses { get; set; }
    
        public virtual DbSet<OperationGroup> OperationGroups { get; set; }
    
        public virtual DbSet<Operation> Operations { get; set; }
    
        public virtual DbSet<OperationGroupLink> OperationGroupLinks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Library>()
                .HasMany(s => s.Dependencies)
                .WithMany(c => c.Dependendends)
                .Map(mm =>
                {
                    mm.ToTable("Library_Library", UserManagementConstants.SchemaName);
                });

            modelBuilder.Entity<Library>()
                .HasMany(s => s.PluginTargets)
                .WithMany(a => a.AvailablePlugins)
                .Map(mm =>
                {
                    mm.ToTable("Library_Application", UserManagementConstants.SchemaName);
                });

            modelBuilder.Entity<Library>()
                .HasMany(s => s.ShellTargets)
                .WithRequired(a => a.Shell);

            modelBuilder.Entity<Library>()
                .HasMany(s => s.AccessPermissions)
                .WithOptional(l => l.Library);

            modelBuilder.Entity<UserGroup>()
                .HasMany(s => s.LibraryAccesses)
                .WithOptional(l => l.UserGroup);

            modelBuilder.Entity<Application>()
                .HasMany(a => a.ApplicationAccesses)
                .WithOptional(aa => aa.Application);

            modelBuilder.Entity<UserGroup>()
                .HasMany(s => s.ApplicationAccesses)
                .WithOptional(l => l.UserGroup);

            modelBuilder.Entity<LibraryAccess>()
                .HasMany(s => s.OperationGroups)
                .WithMany(o => o.LibraryAccesses)
                .Map(mm =>
                {
                    mm.ToTable("LibraryAccess_OperationGroup", UserManagementConstants.SchemaName);
                });

            modelBuilder.Entity<OperationGroup>()
                .HasMany(s => s.EmbeddedGroups)
                .WithMany(o => o.ParentGroups)
                .Map(mm =>
                {
                    mm.ToTable("OpGr_OpGr", UserManagementConstants.SchemaName);
                });

            modelBuilder.Entity<OperationGroup>()
                .HasMany(s => s.OperationGroupLinks)
                .WithOptional(l => l.OperationGroup);

            modelBuilder.Entity<Operation>()
                .HasMany(s => s.OperationGroupLinks)
                .WithOptional(gl => gl.Operation);
        }
    }
}