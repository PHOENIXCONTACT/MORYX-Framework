using System.Threading;
using Marvin.Configuration;
using Marvin.Model;
using Marvin.Model.Npgsql;
using Marvin.TestTools.Test.Model;
using Moq;

namespace NpgsqlTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dbConfig = new NpgsqDatabaseConfig
            {
                Database = "NpgsqlTest",
                Username = "postgres",
                Password = "postgres",
                Port = 5432,
                Host = "localhost"
            };

            var configManagerMock = new Mock<IConfigManager>();
            configManagerMock.Setup(c => c.GetConfiguration<NpgsqDatabaseConfig>(It.IsAny<string>())).Returns(dbConfig);
            
            var factory = new TestModelUnitOfWorkFactory { ConfigManager = configManagerMock.Object };
            factory.Initialize();

            var modelConfigurator = ((IModelConfiguratorFactory) factory).GetConfigurator();

            var uow = factory.Create();
            var houseRepository = uow.GetRepository<IHouseEntityRepository>();

            var houses = houseRepository.GetAll();

            var newHouse = houseRepository.Create();
            
            houseRepository.UnitOfWork.Save();

            Thread.Sleep(2000);

            newHouse.Name = "1^21312341";

            houseRepository.UnitOfWork.Save();

            //Thread.Sleep(2000);

            //houseRepository.Remove(newHouse);

            //houseRepository.UnitOfWork.Save();

            /*
            if (modelConfigurator.TestConnection(dbConfig))
            {
                //modelConfigurator.UpdateDatabase("201801191129253_InitialCreate");
                modelConfigurator.UpdateDatabase(dbConfig);
            }
            else
            {
                modelConfigurator.CreateDatabase(dbConfig);
                var exists = modelConfigurator.TestConnection(dbConfig);

                var setups = modelConfigurator.GetAllSetups();
                var carSetup = setups.First();

                modelConfigurator.Execute(dbConfig, carSetup, string.Empty);
            }
            */
        }
    }
}
