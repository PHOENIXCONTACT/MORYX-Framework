// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.Setups;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.ControlSystem.SetupProvider.Tests
{
    /// <summary>
    /// Tests for the setup-management
    /// </summary>
    [TestFixture]
    public class SetupManagerTests
    {
        private Mock<ISetupTriggerFactory> _triggerFactoryMock;
        private Mock<IResourceManagement> _resourceManagerMock;
        private ModuleConfig _setupManagerConfig;

        /// <summary>
        /// Initialize the test-environment before every test
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            _triggerFactoryMock = new Mock<ISetupTriggerFactory>();
            _triggerFactoryMock.Setup(tf => tf.Create(It.IsAny<SetupTriggerConfig>())).Returns(
                (SetupTriggerConfig sc) =>
                {
                    SetupTriggerBase<SetupTriggerConfig> result = null;
                    switch (sc.PluginName)
                    {
                        case nameof(TestTriggerPrepare):
                            result = new TestTriggerPrepare();
                            break;
                        case nameof(TestTriggerCleanup):
                            result = new TestTriggerCleanup();
                            break;
                        case nameof(MultiStepTrigger):
                            result = new MultiStepTrigger();
                            break;
                    }
                    result?.Initialize(sc);
                    return result;
                });

            _setupManagerConfig = new ModuleConfig
            {
                SetupTriggers = new List<SetupTriggerConfig>
                {
                    // Single prepare
                    CreateTriggerConfig(nameof(TestTriggerPrepare), 1),
                    // Multi prepare
                    CreateTriggerConfig(nameof(MultiStepTrigger), 2),
                    // Parallel clean-up
                    CreateTriggerConfig(nameof(TestTriggerCleanup), 1),
                    CreateTriggerConfig(nameof(TestTriggerCleanup), 1)
                }
            };

            _resourceManagerMock = new Mock<IResourceManagement>();
            _resourceManagerMock
                .Setup(rmm => rmm.GetResources<ICell>(It.IsAny<ICapabilities>()))
                .Returns(Enumerable.Empty<ICell>());
        }

        [TearDown]
        public void ClearJobList()
        {
        }

        #region Tests

        [Test(Description = "Check, that initialize created the configured triggers")]
        public void TestInitialize()
        {
            // Arrange && Act
            CreateSetupManager(_setupManagerConfig);

            //Assert
            Assert.DoesNotThrow(delegate
            {
                foreach (var triggerConfigs in _setupManagerConfig.SetupTriggers)
                    _triggerFactoryMock.Verify(f => f.Create(triggerConfigs), Times.Once);
            });
        }

        [TestCase(42, 0, Description = "Check that RequiredSetup is empty, if capabilities are present")]
        [TestCase(0, 5, Description = "Check that RequiredSetup returns the right (SetupType.ProvideCapabilities)")]
        public void TestRequiredSetup(int currentState, int steps)
        {
            // Arrange
            var manager = CreateSetupManager(_setupManagerConfig);
            AdjustCurrentResourceCapabilities(currentState);
            IProductionRecipe recipe = CreateRecipe(4711, 42).Object;

            // Act
            var requiredSetup = manager.RequiredSetup(SetupExecution.BeforeProduction, recipe, new CurrentResourceTarget(_resourceManagerMock.Object));

            // Assert
            if (steps > 0)
            {
                Assert.That(requiredSetup.Workplan.Steps.Count(), Is.EqualTo(steps));
            }
            else
                Assert.That(requiredSetup, Is.Null);
        }

        [Test(Description = "There should be only one SetupJob of type cleanup after the only production job. " +
                            "The cleanup Job must have a dependency on the production job.")]
        public void AddOneCleanupJobBehindTheOnlyProductionJob()
        {
            // Arrange
            var manager = CreateSetupManager(_setupManagerConfig);
            AdjustCurrentResourceCapabilities(42);

            var recipe = CreateRecipe(1783, 42).Object;

            // Act
            var setupRecipe = manager.RequiredSetup(SetupExecution.AfterProduction, recipe, new CurrentResourceTarget(_resourceManagerMock.Object));

            // Assert
            Assert.That(setupRecipe.Execution, Is.EqualTo(SetupExecution.AfterProduction), "The classification is not SetupExecution.AfterProduction");
            var workplan = setupRecipe.Workplan;
            // Temporary recipe not executable
            Assert.That(workplan.Steps.Count(), Is.EqualTo(4), "Workplan should have 4 steps: 2 setup steps, split and join");
            Assert.That(workplan.Connectors.Count(), Is.EqualTo(7), "Workplan should have 7 connectors: start, end, failed AND 2 step-in, 2 step-out");
        }

        /// <summary>
        /// Create a SetupManager and initialize those fields, that are usually injected
        /// </summary>
        private SetupManager CreateSetupManager(ModuleConfig config)
        {
            var setupManager = new SetupManager
            {
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                TriggerFactory = _triggerFactoryMock.Object,
                Config = config
            };

            setupManager.Start();

            return setupManager;
        }

        private void AdjustCurrentResourceCapabilities(int setupState)
        {
            _resourceManagerMock
                .Setup(rmm => rmm.GetResources<ICell>(It.IsAny<TestSetupCapabilities>()))
                .Returns<ICapabilities>(capabilities =>
                {
                    var setupCapabilities = (TestSetupCapabilities)capabilities;
                    return setupCapabilities.SetupState == setupState
                        ? [new Mock<ICell>().Object]
                        : Enumerable.Empty<ICell>();
                });
        }

        /// <summary>
        /// Creates an <see cref="IProductRecipe"/>. The implementation also implements <see cref="ITestRecipe"/>
        /// for special test handling
        /// </summary>
        private static Mock<IProductionRecipe> CreateRecipe(long id, int state)
        {
            var productRecipeMock = new Mock<IProductionRecipe>();
            productRecipeMock.SetupGet(r => r.Id).Returns(id);

            var testRecipeMock = productRecipeMock.As<ITestRecipe>();
            testRecipeMock.SetupGet(s => s.SetupState).Returns(state);

            return productRecipeMock;
        }

        /// <summary>
        /// Create a config for the triggers
        /// </summary>
        private static SetupTriggerConfig CreateTriggerConfig(string triggerName, int order)
        {
            var triggerConf = new SetupTriggerConfig();
            triggerConf.PluginName = triggerName;
            triggerConf.SortOrder = order;

            return triggerConf;
        }

        #endregion
    }
}
