using System.Linq;
using System.Threading;
using Marvin.Configuration;
using Marvin.Container;
using Marvin.Model;
using Marvin.Model.PostgreSQL;
using Marvin.TestTools.Test.Inheritance.Model;
using Marvin.TestTools.Test.Model;
using Moq;

namespace NpgsqlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbConfig = new NpgsqlDatabaseConfig
            {
                Database = "NpgsqlTest",
                Username = "postgres",
                Password = "postgres",
                Port = 5432,
                Host = "localhost"
            };

            var configManagerMock = new Mock<IConfigManager>();
            configManagerMock.Setup(c => c.GetConfiguration<NpgsqlDatabaseConfig>(It.IsAny<string>())).Returns(dbConfig);

            var parentFactory = new TestModelUnitOfWorkFactory { ConfigManager = configManagerMock.Object };

            var containerMock = new Mock<IContainer>();
            containerMock.Setup(c => c.Resolve<IUnitOfWorkFactory>(TestModelConstants.Namespace)).Returns(parentFactory);

            parentFactory.Container = containerMock.Object;
            parentFactory.Initialize();

            
            var factory = new InheritedTestModelUnitOfWorkFactory
            {
                Container = containerMock.Object,
                ConfigManager = configManagerMock.Object
            };
            factory.Initialize();

            var modelConfigurator = ((IModelConfiguratorFactory) factory).GetConfigurator();

            modelConfigurator.MigrateDatabase(dbConfig);

        }
    }
}
