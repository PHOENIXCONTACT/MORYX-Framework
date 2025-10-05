// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moryx.Configuration;
using Moryx.Logging;
using Moryx.Runtime.Kernel.Tests.Dummys;
using Moryx.Runtime.Kernel.Tests.ModuleMocks;
using Moryx.Runtime.Modules;
using Moq;
using Moryx.Tools;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;

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
            var moduleManagerConfig = new ModuleManagerConfig {ManagedModules = [] };
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
        public void FacadeCollectionInjection()
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
            moduleManager.StartModules();

            // Assert
            Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
            Assert.That(dependent.Facades.Length, Is.EqualTo(3), "Faulty number of facades");
        }

        [Test]
        public void FacadeCollectionNoEntry()
        {
            // Arrange
            var dependent = new ModuleC();
            var moduleManager = CreateObjectUnderTest([
                dependent
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
            Assert.That(dependent.Facades.Length, Is.EqualTo(0), "Faulty number of facades");
        }

        [Test]
        public void FacadeCollectionSingleEntry()
        {
            // Arrange
            var dependent = new ModuleC();
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                dependent
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Assert.That(dependent.Facades, Is.Not.Null, "No facade injected");
            Assert.That(dependent.Facades.Length, Is.EqualTo(1), "Faulty number of facades");
        }


        [Test]
        public void FacadeInjection()
        {
            // Arrange
            var dependency = new ModuleA();
            var depend = new ModuleADependent();
            var moduleManager = CreateObjectUnderTest([
                dependency,
                depend
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Assert.That(depend.Dependency, Is.Not.Null, "Facade not injected correctly");
        }

        [Test]
        public void ShouldExcludeMissingFacadeAndItsDependents()
        {
            // Arrange
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                new ModuleCSingle(), 
                new ModuleADependent(),
                new ModuleADependentTransient()
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(4));
            var available = moduleManager.DependencyTree.RootModules
                .Flatten(md => md.Dependents).ToList();
            Assert.That(available.Count, Is.EqualTo(2));
        }

        [Test]
        public void ShouldExcludeWhenInCollection()
        {
            // Arrange
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                new ModuleBUsingA(),
                new ModuleC()
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(3));
            var available = moduleManager.DependencyTree.RootModules
                .Flatten(md => md.Dependents).ToList();
            Assert.That(available.Count, Is.EqualTo(2));
        }

        [Test]
        public void ShouldIncludeMissingFacadeInDependencyList()
        {
            // Arrange
            var moduleBUsingA = new ModuleBUsingA();
            var moduleManager = CreateObjectUnderTest([
                new ModuleB1(),
                moduleBUsingA,
                new ModuleC()
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Assert.That(moduleManager.AllModules.Count(), Is.EqualTo(3));
            var moduleBUsingA_Dependencies = moduleManager.StartDependencies(moduleBUsingA);
            Assert.That(moduleBUsingA_Dependencies.Count(), Is.EqualTo(1));
        }

        [Test]
        public void ShouldInitializeTheModule()
        {
            // Arrange
            var mockModule = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest([mockModule.Object]);
            //moduleManager.Initialize();

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

            var moduleManager = CreateObjectUnderTest([
                mockModule1.Object,
                mockModule2.Object
            ]);

            // Act
            moduleManager.StartModules();

            // Assert
            Thread.Sleep(3000); // Give the thread pool some time

            mockModule1.Verify(mock => mock.Initialize(), Times.Exactly(2));
            mockModule1.Verify(mock => mock.Start());

            mockModule2.Verify(mock => mock.Initialize(), Times.Exactly(2));
            mockModule2.Verify(mock => mock.Start());
        }

        [Test]
        public void ShouldStartOneModule()
        {
            // Argange
            var mockModule = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest([mockModule.Object]);

            // Act
            moduleManager.StartModule(mockModule.Object);

            Thread.Sleep(1);

            // Assert
            mockModule.Verify(mock => mock.Initialize());
            mockModule.Verify(mock => mock.Start());
        }

        [Test]
        public void ShouldStopModulesAndDeregisterFromEvents()
        {
            // Argange
            var mockModule1 = new Mock<IServerModule>();
            var mockModule2 = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest([mockModule1.Object, mockModule2.Object]);
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

            var moduleManager = CreateObjectUnderTest([mockModule.Object]);
            moduleManager.ModuleStateChanged += (sender, args) => eventFired = true;

            // Act
            mockModule.Raise(mock => mock.StateChanged += null, null, new ModuleStateChangedEventArgs());

            // Assert
            Assert.That(eventFired, "ModuleManager doesn't observe state changed events of modules.");
        }

        [Test]
        public void CheckLifeCycleBoundActivatedCountIs1()
        {
            // Argange
            var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
            var moduleManager = CreateObjectUnderTest([module]);

            // Act
            moduleManager.StartModules();

            WaitForTimeboxed(() => module.State == ServerModuleState.Running);

            // Assert
            Assert.That(module.ActivatedCount, Is.EqualTo(1));
        }

        [Test]
        public void CheckLifeCycleBoundDeactivatedCountIs1()
        {
            // Argange
            var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
            var moduleManager = CreateObjectUnderTest([module]);

            // Act
            moduleManager.StartModules();

            WaitForTimeboxed(() => module.State == ServerModuleState.Running);

            moduleManager.StopModules();

            WaitForTimeboxed(() => module.State == ServerModuleState.Stopped);

            // Assert
            Assert.That(module.ActivatedCount, Is.EqualTo(1));
        }

        private static void WaitForTimeboxed(Func<bool> condition, int maxSeconds = 10) {
            var i = 0;
            while (!condition() && (i < maxSeconds))
            {
                Thread.Sleep(100);
                i++;
            }
        }
    }
}
