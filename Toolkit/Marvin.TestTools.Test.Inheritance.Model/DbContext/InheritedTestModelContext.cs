using System.Data.Common;
using System.Data.Entity;
using Marvin.Model;
using Marvin.TestTools.Test.Model;

namespace Marvin.TestTools.Test.Inheritance.Model
{
    public class InheritedTestModelContext : TestModelContext
    {
        public InheritedTestModelContext()
        {
        }

        public InheritedTestModelContext(string connectionString, ContextMode mode) : base(connectionString, mode)
        {
        }

        public InheritedTestModelContext(DbConnection connection, ContextMode mode) : base(connection, mode)
        {
        }

        public virtual DbSet<SuperCarEntity> SuperCars { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SuperCarEntity>()
                .ToTable(nameof(SuperCarEntity));
        }
    }
}