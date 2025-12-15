// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moq;
using Moryx.Drivers.OpcUa.Factories;
using NUnit.Framework;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class DriverPropertiesTests : OpcUaTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await BasicSetup();
        var appConfigFactoryMock = new Mock<ApplicationConfigurationFactory>();
        appConfigFactoryMock
            .Setup(f => f.Create(It.IsAny<ILogger>(), It.IsAny<string>()))
            .Returns(Task.FromResult(new ApplicationConfiguration { ApplicationName = "TestApp" }));

        _driver.ApplicationConfigurationFactory = appConfigFactoryMock.Object;
    }

    [Test(Description = "Use Aliases for node Ids")]
    public void DriverDoesntCrashWithInvalidServerUrl()
    {
        //Arrange
        _driver.OpcUaServerUrl = "";

        void Action() => _driver.TryConnect(true).GetAwaiter().GetResult();

        //Assert
        Assert.DoesNotThrow(Action);
    }
}
