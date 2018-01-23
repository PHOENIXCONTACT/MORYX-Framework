using System.Collections.Generic;
using System.Linq;
using Marvin.Configuration;
using Marvin.Logging;
using Marvin.Runtime.Kernel.Tests.ModuleMocks;
using Marvin.Runtime.Modules;
using Moq;
using NUnit.Framework;

namespace Marvin.Runtime.Kernel.Tests
{
    [TestFixture]
    public class ModuleManagerTests
    {
        private Mock<IConfigManager> _mockConfigManager;
        private Mock<ILoggerManagement> _mockLoggerManagement;

        [SetUp]
        public void Setup()
        {
            _mockConfigManager = new Mock<IConfigManager>();
            var moduleManagerConfig = new ModuleManagerConfig {ManagedModules = new List<ManagedModuleConfig>()};
            _mockConfigManager.Setup(mock => mock.GetConfiguration<ModuleManagerConfig>()).Returns(moduleManagerConfig);

            _mockLoggerManagement = new Mock<ILoggerManagement>();
            var mockLogger = new Mock<IModuleLogger>();
            _mockLoggerManagement.Setup(mock => mock.ActivateLogging(It.IsAny<ILoggingHost>()))
                .Callback((ILoggingHost par) => par.Logger = mockLogger.Object);
        }


        private ModuleManager CreateObjectUnderTest(IServerModule[] modules)
        {
            return new ModuleManager
            {
                ServerModules = modules,
                ConfigManager = _mockConfigManager.Object,
                LoggerManagement = _mockLoggerManagement.Object
            };
        }

        [Test]
        public void FacadeCollectionInjection()
        {
            // Arrange
            var dependend = new ModuleC();
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                new ModuleB1(),
                new ModuleB2(),
                new ModuleB3(),
                dependend
            });


            // Act
            moduleManager.Initialize();

            // Assert
            Assert.NotNull(dependend.Facades, "No facade injected");
            Assert.AreEqual(3, dependend.Facades.Length, "Faulty number of facades");
        }

        [Test]
        public void FacadeCollectionNoEntry()
        {
            // Arrange
            var dependend = new ModuleC();
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                dependend
            });

            // Act
            moduleManager.Initialize();

            // Assert
            Assert.NotNull(dependend.Facades, "No facade injected");
            Assert.AreEqual(0, dependend.Facades.Length, "Faulty number of facades");
        }

        [Test]
        public void FacadeCollectionSingleEntry()
        {
            // Arrange
            var dependend = new ModuleC();
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                new ModuleB1(),
                dependend
            });

            // Act
            moduleManager.Initialize();

            // Assert
            Assert.NotNull(dependend.Facades, "No facade injected");
            Assert.AreEqual(1, dependend.Facades.Length, "Faulty number of facades");
        }


        [Test]
        public void FacadeInjection()
        {
            // Arrange
            var dependency = new ModuleA();
            var depend = new ModuleADependend();
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                dependency,
                depend
            });

            // Act
            moduleManager.Initialize();

            // Assert
            Assert.NotNull(depend.Dependency, "Facade not injected correctly");
        }

        [Test]
        public void ShouldInitializeTheModule()
        {
            // Arrange
            var mockModule = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest(new[] {mockModule.Object});
            moduleManager.Initialize();

            // Act
            moduleManager.InitializeModule(mockModule.Object);

            // Assert
            mockModule.Verify(mock => mock.Initialize());
        }

        [Test]
        public void ShouldStartAllModules()
        {
            // Argange
            var mockModule1 = new Mock<IServerModule>();
            var mockModule2 = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest(new[]
            {
                mockModule1.Object,
                mockModule2.Object
            });
            moduleManager.Initialize();

            // Act
            moduleManager.StartModules();

            // Assert
            mockModule1.Verify(mock => mock.Initialize(), Times.Once);
            mockModule1.Verify(mock => mock.Start());

            mockModule2.Verify(mock => mock.Initialize(), Times.Once);
            mockModule2.Verify(mock => mock.Start());
        }

        [Test]
        public void ShouldStartOneModule()
        {
            // Argange
            var mockModule = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest(new[] {mockModule.Object});
            moduleManager.Initialize();

            // Act
            moduleManager.StartModule(mockModule.Object);

            // Assert
            mockModule.Verify(mock => mock.Initialize(), Times.Once);
            mockModule.Verify(mock => mock.Start());
        }

        [Test]
        public void ShouldStopModulesAndDeregisterFromEvents()
        {
            // Argange
            var mockModule1 = new Mock<IServerModule>();
            var mockModule2 = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest(new[] {mockModule1.Object, mockModule2.Object});
            moduleManager.Initialize();
            moduleManager.StartModules();

            // Act
            moduleManager.StopModules();

            // Assert
            mockModule1.Verify(mock => mock.Stop());
            mockModule2.Verify(mock => mock.Stop());
        }

        [Test]
        public void ShouldObserveModuleStatesAfterInitialize()
        {
            // Argange
            var mockModule = new Mock<IServerModule>();
            var eventFired = false;

            var moduleManager = CreateObjectUnderTest(new[] { mockModule.Object });
            moduleManager.ModuleStateChanged += (sender, args) => eventFired = true;
            moduleManager.Initialize();

            // Act
            moduleManager.Initialize();
            mockModule.Raise(mock => mock.StateChanged += null, null, new ModuleStateChangedEventArgs());

            // Assert
            Assert.IsTrue(eventFired, "ModuleManager doesn't observe state changed events of modules.");
        }
    }
}