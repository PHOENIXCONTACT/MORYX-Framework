// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moq.Protected;
using Moryx.ProcessData.Listener;
using NUnit.Framework;
using Moryx.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Moryx.ProcessData.Tests
{
    [TestFixture]
    public class ProcessDataListenerTests
    {
        private ProcessDataListenerConfig _listenerConfig;
        private Mock<ProcessDataListenerBase> _listenerMock;

        [SetUp]
        public void Setup()
        {
            _listenerMock = new Mock<ProcessDataListenerBase>()
            {
                CallBase = true
            };

            _listenerMock.Object.Logger = new ModuleLogger("Dummy", new NullLoggerFactory());

            _listenerConfig = new ProcessDataListenerConfig();
            _listenerMock.Object.Initialize(_listenerConfig);
            _listenerMock.Object.Start();
        }

        [Test(Description = "Unconfigured measurements should be ignored.")]
        public void UnconfiguredMeasurementShouldBeIgnored()
        {
            // Arrange

            // Act
            _listenerMock.Object.MeasurementAdded(new Measurement("Foo"));

            // Assert
            _listenerMock.Protected().Verify("OnMeasurementAdded", Times.Never(), ItExpr.IsAny<Measurement>());
        }

        [Test(Description = "Configured measurements should not be ignored.")]
        public void ConfiguredMeasurementShouldNotBeIgnored()
        {
            // Arrange
            const string testMeasurand = "HelloWorld";
            _listenerConfig.MeasurandConfigs.Add(new MeasurandConfig { Name = testMeasurand, IsEnabled = true });

            // Act
            _listenerMock.Object.MeasurementAdded(new Measurement(testMeasurand));

            // Assert
            _listenerMock.Protected().Verify("OnMeasurementAdded", Times.Once(), ItExpr.IsAny<Measurement>());
        }
    }
}
