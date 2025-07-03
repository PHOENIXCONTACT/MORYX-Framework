using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.ProcessEngine.Model;
using Moryx.Model.Repositories;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class JobStorageSqliteTests : JobStorageTests
    {
        /// <inheritdoc />
        protected override UnitOfWorkFactory<ProcessContext> BuildUnitOfWorkFactory()
        {
            return InMemoryUnitOfWorkFactoryBuilder
                .Sqlite<ProcessContext>()
                .EnsureDbIsCreated();
        }
    }
}