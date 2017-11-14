using Marvin.Runtime.Kernel.Tests.Mocks;
using Marvin.Runtime.Kernel.Tests.ModuleMocks;
using Marvin.Runtime.Modules;
using Marvin.TestTools.SystemTest.Mocks;
using NUnit.Framework;

namespace Marvin.Runtime.Kernel.Tests
{
    [TestFixture]
    public class ModuleManagerTests
    {
        private ModuleManager _moduleManager;

        [SetUp]
        public void Setup()
        {
            _moduleManager = new ModuleManager
            {
                ConfigManager = new ConfigManagerMock(),
                LoggerManagement = new LoggerManagementMock(),
            };
        }

        [Test]
        public void FacadeInjection()
        {
            // Arrange
            var dependency = new ModuleA();
            var depend = new ModuleADependend();
            _moduleManager.ServerModules = new IServerModule[]
            {
                dependency,
                depend
            };

            // Act
            _moduleManager.Initialize();

            // Assert
            Assert.NotNull(depend.Dependency, "Facade not injected correctly");
        }

        [Test]
        public void FacadeCollectionInjection()
        {
            // Arrange
            var dependend = new ModuleC();
            _moduleManager.ServerModules = new IServerModule[]
            {
                new ModuleB1(),
                new ModuleB2(),
                new ModuleB3(),
                dependend
            };

            // Act
            _moduleManager.Initialize();

            // Assert
            Assert.NotNull(dependend.Facades, "No facade injected");
            Assert.AreEqual(3, dependend.Facades.Length, "Faulty number of facades");
        }

        [Test]
        public void FacadeCollectionSingleEntry()
        {
            // Arrange
            var dependend = new ModuleC();
            _moduleManager.ServerModules = new IServerModule[]
            {
                new ModuleB1(),
                dependend
            };

            // Act
            _moduleManager.Initialize();

            // Assert
            Assert.NotNull(dependend.Facades, "No facade injected");
            Assert.AreEqual(1, dependend.Facades.Length, "Faulty number of facades");
        }

        [Test]
        public void FacadeCollectionNoEntry()
        {
            // Arrange
            var dependend = new ModuleC();
            _moduleManager.ServerModules = new IServerModule[]
            {
                dependend
            };

            // Act
            _moduleManager.Initialize();

            // Assert
            Assert.NotNull(dependend.Facades, "No facade injected");
            Assert.AreEqual(0, dependend.Facades.Length, "Faulty number of facades");
        }
    }
}