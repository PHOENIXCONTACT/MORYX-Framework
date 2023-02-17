// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.ModuleMocks;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using NUnit.Framework;
using System.IO;
using System.Linq;

namespace Moryx.Runtime.Kernel.Tests
{
    [TestFixture]
    public class KernelServiceCollectionExtensionsTests
    {
        private IServiceCollection _serviceCollection;

        [SetUp]
        public void Setup()
        {
            _serviceCollection = new ServiceCollection();
        }

        [Test]
        public void AddsOrInitializesAllKernelComponents()
        {
            // Arrange
            _serviceCollection.AddLogging();

            // Act
            _serviceCollection.AddMoryxKernel();

            // Assert
            var provider = _serviceCollection.BuildServiceProvider();
            var configManager = _serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(ConfigManager));
            var moduleManager = _serviceCollection.FirstOrDefault(s => s.ServiceType == typeof(ModuleManager));
            var parallelOperations = provider.GetRequiredService(typeof(IParallelOperations));
            var moduleContainerFactory = provider.GetRequiredService(typeof(IModuleContainerFactory));
            var currentPlatform = Platform.Current;
            Assert.NotNull(configManager, "Config Manager not resolvable");
            Assert.NotNull(moduleManager, "Module Manager not resolvable");
            Assert.NotNull(parallelOperations, "Parallel Operations not resolvable");
            Assert.NotNull(moduleContainerFactory, "Module Container Factory not resolvable");
            Assert.NotNull(currentPlatform.PlatformName);
            Assert.NotNull(currentPlatform.PlatformVersion);
            Assert.NotNull(currentPlatform.ProductName);
            Assert.NotNull(currentPlatform.ProductVersion);
            Assert.NotNull(currentPlatform.ProductDescription);
        }

        [Test]
        public void AddAllMoryxServerModules()
        {
            // Arrange

            // Act
            _serviceCollection.AddMoryxModules();

            // Assert
            Assert.DoesNotThrow(() => _serviceCollection.Single(s => s.ServiceType == typeof(LifeCycleBoundFacadeTestModule)));
            Assert.DoesNotThrow(() => _serviceCollection.Single(s => s.ServiceType == typeof(ServerModuleA)));
            Assert.DoesNotThrow(() => _serviceCollection.Single(s => s.ServiceType == typeof(IFacadeA)));
        }

        [Test]
        public void SetupConfigurationToBeUsed()
        {
            // Arrange
            var directoryPath = "TestDirectory";
            if (Directory.Exists(directoryPath))
                Directory.Delete(directoryPath);

            _serviceCollection.AddMoryxKernel();
            var provider = _serviceCollection.BuildServiceProvider();

            // Act
            provider.UseMoryxConfigurations(directoryPath);

            // Assert
            Assert.AreEqual(directoryPath, provider.GetRequiredService<ConfigManager>().ConfigDirectory);
            Assert.IsTrue(Directory.Exists(directoryPath));
        }

        [Test]
        public void StartAllModules()
        {
            // Arrange
            var moduleManagerMock = new Mock<IModuleManager>();
            _serviceCollection.AddSingleton<IModuleManager>(moduleManagerMock.Object);
            var provider = _serviceCollection.BuildServiceProvider();

            // Act
            provider.StartMoryxModules();

            // Assert
            moduleManagerMock.Verify(m => m.StartModules(), Times.Once);
            moduleManagerMock.VerifyNoOtherCalls();
        }

        [Test]
        public void StopAllModules()
        {
            // Arrange
            var moduleManagerMock = new Mock<IModuleManager>();
            _serviceCollection.AddSingleton<IModuleManager>(moduleManagerMock.Object);
            var provider = _serviceCollection.BuildServiceProvider();

            // Act
            provider.StopMoryxModules();

            // Assert
            moduleManagerMock.Verify(m => m.StopModules(), Times.Once);
            moduleManagerMock.VerifyNoOtherCalls();
        }
    }
}
