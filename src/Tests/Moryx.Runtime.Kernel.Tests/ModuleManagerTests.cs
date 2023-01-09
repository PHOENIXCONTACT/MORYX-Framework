// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
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
            var moduleManagerConfig = new ModuleManagerConfig {ManagedModules = new List<ManagedModuleConfig>()};
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
            var dependend = new ModuleC();
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                new ModuleB1(),
                new ModuleB2(),
                new ModuleB3(),
                dependend
            });


            // Act
            //moduleManager.Initialize();

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
            //moduleManager.Initialize();

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
            //moduleManager.Initialize();

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
            //moduleManager.Initialize();

            // Assert
            Assert.NotNull(depend.Dependency, "Facade not injected correctly");
        }

        [Test]
        public void ShouldExcludeMissingFacadeAndItsDependends()
        {
            // Arrange
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                new ModuleB1(),
                new ModuleCSingle(), 
                new ModuleADependend(),
                new ModuleADependendTransient()
            });

            // Act
            //moduleManager.Initialize();

            // Assert
            Assert.AreEqual(4, moduleManager.AllModules.Count());
            var available = moduleManager.DependencyTree.RootModules
                .Flatten(md => md.Dependends).ToList();
            Assert.AreEqual(2, available.Count);
        }

        [Test]
        public void ShouldExcludeWhenInCollection()
        {
            // Arrange
            var moduleManager = CreateObjectUnderTest(new IServerModule[]
            {
                new ModuleB1(),
                new ModuleBUsingA(),
                new ModuleC()
            });

            // Act
            //moduleManager.Initialize();

            // Assert
            Assert.AreEqual(3, moduleManager.AllModules.Count());
            var available = moduleManager.DependencyTree.RootModules
                .Flatten(md => md.Dependends).ToList();
            Assert.AreEqual(1, available.Count);
        }

        [Test]
        public void ShouldInitializeTheModule()
        {
            // Arrange
            var mockModule = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest(new[] {mockModule.Object});
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

            var moduleManager = CreateObjectUnderTest(new[]
            {
                mockModule1.Object,
                mockModule2.Object
            });
            //moduleManager.Initialize();

            // Act
            moduleManager.StartModules();

            // Assert
            Thread.Sleep(3000); // Give the thread pool some time

            mockModule1.Verify(mock => mock.Initialize(), Times.Exactly(2));
            mockModule1.Verify(mock => mock.Start());

            mockModule2.Verify(mock => mock.Initialize(), Times.Exactly(2));
            mockModule2.Verify(mock => mock.Start());
        }

        //[Test]
        //public void ShouldStartOneModule()
        //{
        //    // Argange
        //    var mockModule = new Mock<IServerModule>();

        //    var moduleManager = CreateObjectUnderTest(new[] {mockModule.Object});
        //    moduleManager.Initialize();

        //    // Act
        //    moduleManager.StartModule(mockModule.Object);

        //    // Assert
        //    mockModule.Verify(mock => mock.Initialize(), Times.Exactly(2));
        //    mockModule.Verify(mock => mock.Start());
        //}

        [Test]
        public void ShouldStopModulesAndDeregisterFromEvents()
        {
            // Argange
            var mockModule1 = new Mock<IServerModule>();
            var mockModule2 = new Mock<IServerModule>();

            var moduleManager = CreateObjectUnderTest(new[] {mockModule1.Object, mockModule2.Object});
            //moduleManager.Initialize();
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

            // Act
            //moduleManager.Initialize();
            mockModule.Raise(mock => mock.StateChanged += null, null, new ModuleStateChangedEventArgs());

            // Assert
            Assert.IsTrue(eventFired, "ModuleManager doesn't observe state changed events of modules.");
        }

        [Test]
        public void CheckLifeCycleBoundActivatedCountIs1()
        {
            // Argange
            var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
            var moduleManager = CreateObjectUnderTest(new[] { module });

            // Act
            //moduleManager.Initialize();
            moduleManager.StartModules();

            var i = 0;
            WaitForTimeboxed(() => module.State == ServerModuleState.Running);

            // Assert
            Assert.AreEqual(1, module.ActivatedCount);
        }

        [Test]
        public void CheckLifeCycleBoundDeactivatedCountIs1()
        {
            // Argange
            var module = CreateLifeCycleBoundFacadeTestModuleUnderTest();
            var moduleManager = CreateObjectUnderTest(new[] { module });

            // Act
            //moduleManager.Initialize();
            moduleManager.StartModules();

            WaitForTimeboxed(() => module.State == ServerModuleState.Running);

            moduleManager.StopModules();

            WaitForTimeboxed(() => module.State == ServerModuleState.Stopped);

            // Assert
            Assert.AreEqual(1, module.ActivatedCount);
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
