// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Moryx.Container;
using Moryx.Runtime.Kernel.Tests.ModuleMocks;
using Moryx.Runtime.Modules;
using Moryx.Threading;
using NUnit.Framework;

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
            Assert.That(configManager, Is.Not.Null, "Config Manager not resolvable");
            Assert.That(moduleManager, Is.Not.Null, "Module Manager not resolvable");
            Assert.That(parallelOperations, Is.Not.Null, "Parallel Operations not resolvable");
            Assert.That(moduleContainerFactory, Is.Not.Null, "Module Container Factory not resolvable");
            Assert.That(currentPlatform.PlatformName, Is.Not.Null);
            Assert.That(currentPlatform.PlatformVersion, Is.Not.Null);
            Assert.That(currentPlatform.ProductName, Is.Not.Null);
            Assert.That(currentPlatform.ProductVersion, Is.Not.Null);
            Assert.That(currentPlatform.ProductDescription, Is.Not.Null);
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
            Assert.That(provider.GetRequiredService<ConfigManager>().ConfigDirectory, Is.EqualTo(directoryPath));
            Assert.That(Directory.Exists(directoryPath));
        }

        [Test]
        public async Task StartAndStopAllModules()
        {
            // Arrange
            var moduleManagerMock = new Mock<IModuleManager>();
            _serviceCollection.AddSingleton(moduleManagerMock.Object);
            var provider = _serviceCollection.BuildServiceProvider();
            var moduleManager = provider.GetRequiredService<IModuleManager>();

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            moduleManagerMock.Verify(m => m.StartModulesAsync(), Times.Once);
            moduleManagerMock.VerifyNoOtherCalls();

            // Act
            await moduleManager.StopModulesAsync();

            // Assert
            moduleManagerMock.Verify(m => m.StopModulesAsync(), Times.Once);
        }
    }
}
