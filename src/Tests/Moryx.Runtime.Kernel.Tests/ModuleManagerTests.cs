// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moryx.Configuration;
using Moryx.Logging;
using Moryx.Runtime.Kernel.Tests.ModuleMocks;
using Moryx.Runtime.Modules;
using Moq;
using Moryx.Tools;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Runtime.Kernel.Tests.Dummies;

namespace Moryx.Runtime.Kernel.Tests
{
    [TestFixture]
    public class ModuleManagerTests
    {
        private Mock<IConfigManager> _mockConfigManager;
        private Mock<IModuleLogger> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockConfigManager = new Mock<IConfigManager>();
            var moduleManagerConfig = new ModuleManagerConfig { ManagedModules = [] };
            _mockConfigManager.Setup(mock => mock.GetConfiguration(typeof(ModuleManagerConfig), typeof(ModuleManagerConfig).FullName, false)).Returns(moduleManagerConfig);
            _mockConfigManager.Setup(mock => mock.GetConfiguration(typeof(RuntimeConfigManagerTestConfig2), typeof(RuntimeConfigManagerTestConfig2).FullName, false)).Returns(new RuntimeConfigManagerTestConfig2());

            _mockLogger = new Mock<IModuleLogger>();
            _mockLogger.Setup(ml => ml.GetChild(It.IsAny<string>(), It.IsAny<Type>())).Returns(_mockLogger.Object);
        }

        private ModuleManager CreateObjectUnderTest(IServerModule[] modules)
        {
            return new ModuleManager(modules, _mockConfigManager.Object, new NullLogger<ModuleManager>());
        }

        private LifeCycleBoundFacadeTestModule CreateLifeCycleBoundFacadeTestModuleUnderTest()
        {
            return new LifeCycleBoundFacadeTestModule(new ModuleContainerFactory(), _mockConfigManager.Object, new NullLoggerFactory());
        }

        [Test]
        public async Task FacadeCollectionInjection()
        {
            // Arrange
            var dependent = new ModuleC();
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                new ModuleB2(),
                new ModuleB3(),
                dependent
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
            Assert.That(dependent.Facades.Length, Is.EqualTo(3), "Faulty number of facades");
        }

        [Test]
        public async Task FacadeCollectionNoEntry()
        {
            // Arrange
            var dependent = new ModuleC();
            var moduleManager = CreateObjectUnderTest([
                dependent
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
            Assert.That(dependent.Facades.Length, Is.EqualTo(0), "Faulty number of facades");
        }

        [Test]
        public async Task FacadeCollectionSingleEntry()
        {
            // Arrange
            var dependent = new ModuleC();
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                dependent
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
            Assert.That(dependent.Facades.Length, Is.EqualTo(1), "Faulty number of facades");
        }

        [Test]
        public async Task FacadeInjection()
        {
            // Arrange
            var dependency = new ModuleA();
            var depend = new ModuleADependent();
            var moduleManager = CreateObjectUnderTest([
                dependency,
                depend
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(depend.Dependency, Is.Not.Null, "Facade not injected correctly");
        }

        [Test]
        public async Task ShouldExcludeMissingFacadeAndItsDependents()
        {
            // Arrange
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                new ModuleCSingle(),
                new ModuleADependent(),
                new ModuleADependentTransient()
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(4));
            var available = moduleManager.DependencyTree.RootModules
                .Flatten(md => md.Dependents).ToList();
            Assert.That(available.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldExcludeWhenInCollection()
        {
            // Arrange
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                new ModuleBUsingA(),
                new ModuleC()
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(3));
            var available = moduleManager.DependencyTree.RootModules
                .Flatten(md => md.Dependents).ToList();
            Assert.That(available.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task ShouldIncludeMissingFacadeInDependencyList()
        {
            // Arrange
            var moduleBUsingA = new ModuleBUsingA();
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                moduleBUsingA,
                new ModuleC()
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(3));
            var moduleBUsingADependencies = moduleManager.StartDependencies(moduleBUsingA);
            Assert.That(moduleBUsingADependencies.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task ShouldInitializeTheModule()
        {
            // Arrange
            var mockModule = new Mock<IServerModule>();
            var moduleManager = CreateObjectUnderTest([mockModule.Object]);

            // Act
            await moduleManager.InitializeModuleAsync(mockModule.Object);

            // Assert
            mockModule.Verify(mock => mock.InitializeAsync());
        }

        [Test]
        public async Task ShouldStartAllModules()
        {
            // Arrange
            var mockModule1 = new Mock<IServerModule>();
            var mockModule2 = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest([
                mockModule1.Object,
                mockModule2.Object
            ]);

            // Act
            await moduleManager.StartModulesAsync();

            // Assert
            Thread.Sleep(3000); // Give the thread pool some time

            mockModule1.Verify(mock => mock.InitializeAsync(), Times.Exactly(2));
            mockModule1.Verify(mock => mock.StartAsync());

            mockModule2.Verify(mock => mock.InitializeAsync(), Times.Exactly(2));
            mockModule2.Verify(mock => mock.StartAsync());
        }

        [Test]
        public async Task ShouldStartOneModule()
        {
            // Arrange
            var mockModule = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest([mockModule.Object]);

            // Act
            await moduleManager.StartModuleAsync(mockModule.Object);

            Thread.Sleep(1);

            // Assert
            mockModule.Verify(mock => mock.InitializeAsync());
            mockModule.Verify(mock => mock.StartAsync());
        }

        [Test]
        public async Task ShouldStopModulesAndDeregisterFromEvents()
        {
            // Arrange
            var mockModule1 = new Mock<IServerModule>();
            var mockModule2 = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest([mockModule1.Object, mockModule2.Object]);
            await moduleManager.StartModulesAsync();

            // Act
            await moduleManager.StopModulesAsync();

            // Assert
            mockModule1.Verify(mock => mock.StopAsync());
            mockModule2.Verify(mock => mock.StopAsync());
        }

        [Test]
        public void ShouldObserveModuleStatesAfterInitialize()
        {
            // Arrange
            var mockModule = new Mock<IServerModule>();
            var eventFired = false;

            var moduleManager = CreateObjectUnderTest([mockModule.Object]);
            moduleManager.ModuleStateChanged += (_, _) => eventFired = true;

            // Act
            mockModule.Raise(mock => mock.StateChanged += null, mockModule.Object, new ModuleStateChangedEventArgs());

            // Assert
            Assert.That(eventFired, "ModuleManager doesn't observe state changed events of modules.");
        }

        [Test]
        public async Task CheckLifeCycleBoundActivatedCountIs1()
        {
            // Arrange
            var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
            var moduleManager = CreateObjectUnderTest([module]);

            // Act
            await moduleManager.StartModulesAsync();

            WaitForTimeboxed(() => module.State == ServerModuleState.Running);

            // Assert
            Assert.That(module.ActivatedCount, Is.EqualTo(1));
        }

        [Test]
        public async Task CheckLifeCycleBoundDeactivatedCountIs1()
        {
            // Arrange
            var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
            var moduleManager = CreateObjectUnderTest([module]);

            // Act
            await moduleManager.StartModulesAsync();

            WaitForTimeboxed(() => module.State == ServerModuleState.Running);

            await moduleManager.StopModulesAsync();

            WaitForTimeboxed(() => module.State == ServerModuleState.Stopped);

            // Assert
            Assert.That(module.ActivatedCount, Is.EqualTo(1));
        }

        private static void WaitForTimeboxed(Func<bool> condition, int maxSeconds = 10)
        {
            var i = 0;
            while (!condition() && (i < maxSeconds))
            {
                Thread.Sleep(100);
                i++;
            }
        }
    }
}
