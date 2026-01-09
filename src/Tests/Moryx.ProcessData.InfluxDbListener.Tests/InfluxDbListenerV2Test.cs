// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Moq;
using Moryx.Logging;
using Moryx.ProcessData.Listener;
using Moryx.TestTools.UnitTest;
using Moryx.Tools;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace Moryx.ProcessData.InfluxDbListener.Tests;

[TestFixture(true)]
[TestFixture(false)]
public class InfluxDbListenerV2Test
{
    private const string Username = "username";
    private const string Password = "password";
    private const string MeasurandName = "Test";
    private const int ReportInterval = 10;
    private const int Retries = 3;
    private const int Port = 56789;

    private InfluxDbListenerV2 _listener;
    private Mock<IModuleLogger> _moduleLogger;
    private WireMockServer _mockServer;
    private NotSoParallelOps _parallelOperations;
    private readonly InfluxDbListenerConfigV2 _config;

    public InfluxDbListenerV2Test(bool useSsl)
    {
        if (useSsl)
        {
            CheckDevCertAvailabilityAndTrust();
        }

        _config = CreateListenerConfig();
        _config.UseTls = useSsl;
        _config.Proxy = useSsl ? string.Empty : "http://proxy.europe.phoenixcontact.com:8080/";
        _config.BypassProxyOnLocalHost = true;
    }

    private static void CheckDevCertAvailabilityAndTrust()
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "dev-certs https --check --trust",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        var result = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        if (!result.Contains("A trusted certificate was found"))
        {
            throw new IgnoreException("Development certificate is not available or trusted. Skipping all tests.");
        }
    }

    [SetUp]
    public void Setup()
    {
        ReflectionTool.TestMode = true;
        Platform.SetPlatform();
        CreateLogger();
        CreateServerMock();
        CreateParrallelOps();
        CreateInfluxDbV2Listener();
    }

    [TearDown]
    public void Teardown()
    {
        _mockServer?.Stop();
        _mockServer?.Dispose();
        _parallelOperations.Dispose();
    }

    [Test]
    public void Lifecycle_WithDependencies_StartAndStopPlugin()
    {
        // Arrange
        // Act
        // Assert
        _listener.Initialize(_config);
        _listener.Start();
    }

    [TestCase(200, 0, 1, Description = "Successful write operation does not trigger retries or error logs")]
    [TestCase(500, Retries + 1, Retries + 1, Description = "Unsuccessful write operation triggers retries and error logs")]
    public void AddMeasurement_WithFullConfig_CallsWriteApi(int statusCode, int numberOfErrorLogs, int numberOfWriteOps)
    {
        // Arrange
        _listener.Initialize(_config);
        _listener.Start();
        _mockServer.Given(Request.Create().WithPath("/api/v2/write").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(statusCode));

        // Act
        _listener.MeasurementAdded(CreateMeasurement());
        AwaitParrallelOperations();

        // Assert
        VerifyLogs(LogLevel.Error, Times.Exactly(numberOfErrorLogs));
        Assert.That(GetWritesCount(), Is.EqualTo(numberOfWriteOps));
    }

    [TestCase(200, 0, 1, Description = "Successful write operation does not trigger retries or error logs")]
    [TestCase(500, Retries + 1, Retries + 1, Description = "Unsuccessful write operation triggers retries and error logs")]
    public void AddMeasurand_WithFullConfig_CallsWriteApi(int statusCode, int numberOfErrorLogs, int numberOfWriteOps)
    {
        // Arrange
        _listener.Initialize(_config);
        _listener.Start();
        _mockServer.Given(Request.Create().WithPath("/api/v2/write").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(statusCode));

        // Act
        _listener.MeasurandAdded(CreateMeasurand());
        AwaitParrallelOperations();

        // Assert
        VerifyLogs(LogLevel.Error, Times.Exactly(numberOfErrorLogs));
        Assert.That(GetWritesCount(), Is.EqualTo(numberOfWriteOps));
    }

    [Test]
    public void AddMeasurand_WithIncompleteConfig_Fails()
    {
        // Arrange
        var config = CreateListenerConfig();
        config.DatabaseName = null;
        _listener.Initialize(config);
        _listener.Start();
        _mockServer.Given(Request.Create().WithPath("/api/v2/write").UsingPost())
            .RespondWith(Response.Create().WithStatusCode(200));

        // Act
        _listener.MeasurandAdded(CreateMeasurand());
        AwaitParrallelOperations();

        // Assert
        VerifyLogs(LogLevel.Error, Times.Exactly(Retries + 1));
        Assert.That(GetWritesCount(), Is.EqualTo(0));
    }

    private void CreateInfluxDbV2Listener() => _listener = new InfluxDbListenerV2
    {
        ParallelOperations = _parallelOperations,
        Logger = _moduleLogger.Object
    };

    private void CreateServerMock() => _mockServer = WireMockServer.Start(port: Port, useSSL: _config.UseTls);

    private void CreateLogger()
    {
        _moduleLogger = new Mock<IModuleLogger>();
        _moduleLogger.Setup(l => l.GetChild(It.IsAny<string>(), It.IsAny<Type>())).Returns(_moduleLogger.Object);
    }

    private void CreateParrallelOps() => _parallelOperations = new NotSoParallelOps();

    private static InfluxDbListenerConfigV2 CreateListenerConfig() => new()
    {
        PluginName = "InfluxDb Listener",
        Host = "localhost",
        Port = Port,
        UseTls = true,
        MaxNumberOfRetries = Retries,
        Username = Username,
        Password = Password,
        DatabaseName = "database",
        Organisation = "organization",
        ReportIntervalMs = ReportInterval,
        MeasurandConfigs = [new() { IsEnabled = true, Name = MeasurandName }]
    };

    private static Measurand CreateMeasurand()
    {
        var m = new Measurand(MeasurandName);
        m.Measurements.Add(CreateMeasurement());
        m.Measurements.Add(CreateMeasurement());
        return m;
    }

    private static Measurement CreateMeasurement() => new(MeasurandName);

    private int GetWritesCount() => _mockServer.FindLogEntries(Request.Create().WithPath("/api/v2/write")).Count;

    private void AwaitParrallelOperations() => _parallelOperations.WaitForScheduledExecution(ReportInterval * 1000);

    private void VerifyLogs(LogLevel logLevel, Times times) => _moduleLogger.Verify(l =>
        l.Log(logLevel, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), times);
}