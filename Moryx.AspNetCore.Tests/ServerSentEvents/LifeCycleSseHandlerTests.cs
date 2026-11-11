// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Net.ServerSentEvents;
using System.Text.Json;
using System.Threading.Channels;
using Moq;
using Moryx.AspNetCore.ServerSentEvents;
using Moryx.Runtime.Modules;

namespace Moryx.AspNetCore.Tests.ServerSentEvents;

[TestFixture]
public class LifeCycleSseHandlerTests
{
    private Mock<ILifeCycleBoundFacade> _lifeCycleBoundFacade = null;
    private Channel<SseItem<string>> _channel = null;
    private JsonSerializerOptions _serializerOptions = null!;

    [SetUp]
    public void Setup()
    {
        _lifeCycleBoundFacade = new Mock<ILifeCycleBoundFacade>();

        _channel = Channel.CreateUnbounded<SseItem<string>>();

        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    [TestCase(true, true)]
    [TestCase(false, false)]
    public async Task Start_WhenFacadeStateIsSet_SendsExpectedReadinessMessageAndCallsOnReadyAsExpected(
        bool isActivated, bool expectedOnReadyCalled)
    {
        // Arrange
        _lifeCycleBoundFacade.SetupGet(facade => facade.IsActivated).Returns(isActivated);

        var onReadyCalled = false;

        using var handler = new LifeCycleSseHandler(_lifeCycleBoundFacade.Object,
            _channel.Writer, () => onReadyCalled = true, _serializerOptions);

        //Act
        handler.Start();

        var received = await _channel.Reader.ReadAsync();

        var readinessModel = JsonSerializer.Deserialize<EndpointReadinessModel>(
            received.Data, _serializerOptions);

        //Assret
        Assert.Multiple(() =>
        {
            Assert.That(received.EventType, Is.EqualTo(SseEventTypes.Readiness));

            Assert.That(readinessModel, Is.Not.Null);

            Assert.That(readinessModel!.IsReady, Is.EqualTo(isActivated));

            Assert.That(onReadyCalled, Is.EqualTo(expectedOnReadyCalled));
        });
    }
}
