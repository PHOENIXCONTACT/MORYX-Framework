// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.Configuration;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Modules;
using Moryx.Runtime.Tests.Mocks;
using Moryx.Runtime.Tests.Modules;
using NUnit.Framework;

namespace Moryx.Runtime.Tests;

[TestFixture]
public class RegressionTests
{
    private TestModule _moduleUnderTest;
    private Mock<IConfigManager> _configManagerMock;

    [SetUp]
    public void Setup()
    {
        _configManagerMock = new Mock<IConfigManager>();

        _configManagerMock
            .Setup(c => c.GetConfiguration(typeof(TestConfig), It.IsAny<string>(), false))
            .Returns(new TestConfig { Strategy = new StrategyConfig() });

        _moduleUnderTest = new TestModule(
            new ModuleContainerFactory(),
            _configManagerMock.Object,
            new TestLoggerMgmt());
    }

    // ToDo: Check if TransitionTests StoppedToRunning(), StartFromStoppedInitializesAndStarts() and StartFromStoppedInitializeFails() are covered by this Test.
    [Test]
    [Description(
        "Regression test for restarting a stopped module. Modules that are stopped as part of a dependency shutdown chain must be reinitialized before they are started again.")]
    public async Task RestartStoppedModule()
    {
        // Arrange
        var module = (IServerModule)_moduleUnderTest;

        Assert.That(module.State,
            Is.EqualTo(ServerModuleState.Stopped));

        // Act
        await module.StartAsync();

        // Assert
        Assert.That(_moduleUnderTest.InitializeCalls,
            Is.EqualTo(1),
            "Module was not initialized before start.");

        Assert.That(_moduleUnderTest.StartCalls,
            Is.EqualTo(1),
            "Module was not started.");

        Assert.That(module.State,
            Is.EqualTo(ServerModuleState.Running),
            "Module did not enter running state.");
    }
}

