using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.ControlSystem.ProcessEngine.Processes;
using Moryx.Model.Repositories;
using Moryx.Model.Sqlite;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Processes
{
    [TestFixture]
    public class ProcessStorageSqliteFilesystemTests : ProcessStorageTests
    {
        protected override UnitOfWorkFactory<ProcessContext> BuildUnitOfWorkFactory()
        {
            // Create database using `SqliteProcessContext`
            var factory = new UnitOfWorkFactory<SqliteProcessContext>(CreateContextManager());
            var uow = factory
                .Create();

            uow.DbContext.Database.EnsureDeleted();
            uow.DbContext.Database.Migrate();

            return new UnitOfWorkFactory<ProcessContext>(CreateContextManager());
        }

        private SqliteDbContextManager CreateContextManager(string datasource = ".\\test.db")
        {
            SqliteConnectionStringBuilder sqliteConnectionStringBuilder = new SqliteConnectionStringBuilder
            {
                DataSource = datasource
            };
            SqliteConnection sqliteConnection = new SqliteConnection(sqliteConnectionStringBuilder.ConnectionString);

            return new SqliteDbContextManager(sqliteConnection);
        }

        [Test]
        public void NotUsingSqliteSpecificMigrationThrowsException()
        {
            var factory = new UnitOfWorkFactory<ProcessContext>(CreateContextManager(".\\f98ac3f.db"));

            var uow = factory
                .Create();

            uow.DbContext.Database.EnsureDeleted();
            uow.DbContext.Database.Migrate();

            var storage = new ProcessStorage { UnitOfWorkFactory = factory, Logger = null };

            Assert.Throws<SqliteException>(() => storage.Start());
        }
    }
}
