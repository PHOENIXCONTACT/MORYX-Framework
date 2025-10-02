// Copyright (c) 2026, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moq;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using Moryx.AspNetCore.Mqtt.Components;
using System.Threading;
using System.Collections.Generic;
using System;

namespace Moryx.AspNetCore.Mqtt.Tests;

[TestFixture]
public class ManagedMqttClientTests
{
    private Mock<IMqttClient> _mockMqttClient;
    private Mock<IMqttNetLogger> _mockLogger;
    private Mock<IManagedMqttClientStorage> _mockStorage;
    private MqttClientUserOptions _userOptions;
    private ManagedMqttClient _managedClient;

    [SetUp]
    public void Setup()
    {
        _mockMqttClient = new Mock<IMqttClient>();
        _mockLogger = new Mock<IMqttNetLogger>();
        _mockStorage = new Mock<IManagedMqttClientStorage>();

        _userOptions = new MqttClientUserOptions
        {
            MaxPendingMessages = 10,
            PendingMessageStrategy = MqttMessagesOverflowStrategy.RejectNewMessage
        };
        _managedClient = new ManagedMqttClient(_mockMqttClient.Object, _mockLogger.Object, _userOptions);
    }

    [TearDown]
    public void TearDown()
    {
        _managedClient?.Dispose();
    }

    #region IsConnected Tests

    [Test]
    public void IsConnected_WhenInternalClientIsConnected_ReturnsTrue()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);

        Assert.That(_managedClient.IsConnected, Is.True, "IsConnected should return true when the internal client is connected.");
    }

    [Test]
    public void IsConnected_WhenInternalClientIsNotConnected_ReturnsFalse()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(false);

        Assert.That(_managedClient.IsConnected, Is.False, "IsConnected should return false when the internal client is not connected.");
    }

    #endregion

    #region StartAsync Tests

    [Test]
    public async Task StartAsync_NullOptions_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _managedClient.StartAsync(null, CancellationToken.None), "StartAsync should throw ArgumentNullException when options are null.");
    }

    [Test]
    public async Task StartAsync_ValidOptions_ConnectsToMqttBroker()
    {
        var options = new MqttClientOptions();
        var connectResult = new MqttClientConnectResult();
        _mockMqttClient.Setup(c => c.ConnectAsync(It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectResult);
        _mockMqttClient.Setup(c => c.PublishAsync(It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MqttClientPublishResult(null, MqttClientPublishReasonCode.Success, null, []));
        using var cancellation = new CancellationTokenSource();
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(100));

        try
        {
            await _managedClient.StartAsync(options, cancellation.Token);
        }
        catch (OperationCanceledException)
        {
            // Expected - the background task will be cancelled
        }

        _mockMqttClient.Verify(c => c.ConnectAsync(options, It.IsAny<CancellationToken>()), Times.Once,
            "ConnectAsync should be called once with the provided options.");
    }

    [Test]
    public async Task StartAsync_WithStorageManager_LoadsQueuedMessages()
    {
        var optionsWithStorage = new MqttClientUserOptions
        {
            MaxPendingMessages = 10,
            PendingMessageStrategy = MqttMessagesOverflowStrategy.RejectNewMessage,
            MessageStorage = _mockStorage.Object
        };
        var storedMessages = new List<MqttApplicationMessage>
        {
            new MqttApplicationMessageBuilder().WithTopic("test/topic1").WithPayload([1, 2, 3]).Build()
        };
        _mockStorage.Setup(s => s.LoadQueuedMessagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(storedMessages);
        var managedClientWithStorage = new ManagedMqttClient(
            _mockMqttClient.Object,
            _mockLogger.Object,
            optionsWithStorage);
        var options = new MqttClientOptions();
        var connectResult = new MqttClientConnectResult();
        _mockMqttClient.Setup(c => c.ConnectAsync(It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectResult);
        _mockMqttClient.Setup(c => c.PublishAsync(It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MqttClientPublishResult(null, MqttClientPublishReasonCode.Success, null, []));

        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        try
        {
            await managedClientWithStorage.StartAsync(options, cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Expected - the background task will be cancelled
        }

        _mockStorage.Verify(s => s.LoadQueuedMessagesAsync(It.IsAny<CancellationToken>()), Times.Once,
            "LoadQueuedMessagesAsync should be called once during StartAsync when a storage manager is provided.");
        managedClientWithStorage.Dispose();
    }

    #endregion

    #region StopAsync Tests

    [Test]
    public async Task StopAsync_WhenConnected_DisconnectsFromBroker()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);

        await _managedClient.StopAsync(CancellationToken.None);

        _mockMqttClient.Verify(c => c.DisconnectAsync(
            It.IsAny<MqttClientDisconnectOptions>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task StopAsync_CleanDisconnectTrue_UsesNormalDisconnectionReason()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);

        await _managedClient.StopAsync(CancellationToken.None, cleanDisconnect: true);

        _mockMqttClient.Verify(c => c.DisconnectAsync(
            It.Is<MqttClientDisconnectOptions>(o =>
                o.Reason == MqttClientDisconnectOptionsReason.NormalDisconnection),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task StopAsync_CleanDisconnectFalse_UsesUnspecifiedErrorReason()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);

        await _managedClient.StopAsync(CancellationToken.None, cleanDisconnect: false);

        _mockMqttClient.Verify(c => c.DisconnectAsync(
            It.Is<MqttClientDisconnectOptions>(o =>
                o.Reason == MqttClientDisconnectOptionsReason.UnspecifiedError),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region EnqueueAsync Tests

    [Test]
    public async Task EnqueueAsync_NullMessage_ThrowsArgumentNullException()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _managedClient.EnqueueAsync(null, CancellationToken.None),
            "EnqueueAsync should throw ArgumentNullException when the message is null.");
    }

    [Test]
    public async Task EnqueueAsync_BeforeStart_ThrowsInvalidOperationException()
    {
        var message = new MqttApplicationMessage { Topic = "test/topic" };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _managedClient.EnqueueAsync(message, CancellationToken.None),
            "EnqueueAsync should throw InvalidOperationException when called before StartAsync.");
    }

    [Test]
    public async Task EnqueueAsync_ValidMessage_EnqueuesMessage()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);
        var message = new MqttApplicationMessageBuilder().WithTopic("test/topic").WithPayload([1, 2, 3]).Build();

        Assert.DoesNotThrowAsync(async () => await _managedClient.EnqueueAsync(message, CancellationToken.None),
            "EnqueueAsync should not throw when enqueuing a valid message after StartAsync.");
    }

    [Test]
    public async Task EnqueueAsync_QueueFull_RejectNewMessage_SkipsMessage()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);
        MqttApplicationMessage? skippedMessage = null;
        _managedClient.MessageSkipped += (msg) =>
        {
            skippedMessage = msg;
            return Task.CompletedTask;
        };

        // Fill the queue
        for (var i = 0; i < _userOptions.MaxPendingMessages; i++)
        {
            await _managedClient.EnqueueAsync(
                new MqttApplicationMessage { Topic = $"test/topic{i}" },
                CancellationToken.None);
        }
        // Try to add one more message
        var extraMessage = new MqttApplicationMessage { Topic = "test/extra" };
        await _managedClient.EnqueueAsync(extraMessage, CancellationToken.None);

        Assert.That(skippedMessage, Is.Not.Null);
        Assert.That(skippedMessage.Topic, Is.EqualTo("test/extra"));
    }

    #endregion

    #region PingAsync Tests

    [Test]
    public async Task PingAsync_WhenNotConnected_ThrowsInvalidOperationException()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(false);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _managedClient.PingAsync(CancellationToken.None),
            "PingAsync should throw InvalidOperationException when the client is not connected.");
    }

    [Test]
    public async Task PingAsync_WhenConnected_CallsInternalClientPing()
    {
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);
        _mockMqttClient.Setup(c => c.PingAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await _managedClient.PingAsync(CancellationToken.None);

        _mockMqttClient.Verify(c => c.PingAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region SubscribeAsync Tests

    [Test]
    public async Task SubscribeAsync_NullTopicFilters_ThrowsArgumentNullException()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _managedClient.SubscribeAsync(null, CancellationToken.None),
            "SubscribeAsync should throw ArgumentNullException when topic filters are null.");
    }

    [Test]
    public async Task SubscribeAsync_BeforeStart_ThrowsInvalidOperationException()
    {
        var filters = new List<MqttTopicFilter>
        {
            new() { Topic = "test/topic" }
        };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _managedClient.SubscribeAsync(filters, CancellationToken.None),
            "SubscribeAsync should throw InvalidOperationException when called before StartAsync.");
    }

    [Test]
    public async Task SubscribeAsync_WhenConnected_SubscribesToTopics()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);
        var filters = new List<MqttTopicFilter>
        {
            new() { Topic = "test/topic1" },
            new() { Topic = "test/topic2" }
        };

        await _managedClient.SubscribeAsync(filters, CancellationToken.None);

        _mockMqttClient.Verify(c => c.SubscribeAsync(
            It.Is<MqttClientSubscribeOptions>(o => o.TopicFilters.Count == 2),
            It.IsAny<CancellationToken>()), Times.Once,
            "SubscribeAsync should call internal client's SubscribeAsync with correct topic filters when connected.");
    }

    [Test]
    public async Task SubscribeAsync_WhenNotConnected_TracksSubscriptionForReconnect()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);
        _mockMqttClient.Setup(c => c.IsConnected).Returns(false);
        var filters = new List<MqttTopicFilter>
        {
            new() { Topic = "test/topic" }
        };

        await _managedClient.SubscribeAsync(filters, CancellationToken.None);

        _mockMqttClient.Verify(c => c.SubscribeAsync(
            It.IsAny<MqttClientSubscribeOptions>(),
            It.IsAny<CancellationToken>()), Times.Never,
            "SubscribeAsync should not call internal client's SubscribeAsync when not connected.");
    }

    #endregion

    #region UnsubscribeAsync Tests

    [Test]
    public async Task UnsubscribeAsync_NullTopics_ThrowsArgumentNullException()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _managedClient.UnsubscribeAsync(null, CancellationToken.None),
            "UnsubscribeAsync should throw ArgumentNullException when topics are null.");
    }

    [Test]
    public async Task UnsubscribeAsync_BeforeStart_ThrowsInvalidOperationException()
    {
        var topics = new List<string> { "test/topic" };

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _managedClient.UnsubscribeAsync(topics, CancellationToken.None),
            "UnsubscribeAsync should throw InvalidOperationException when called before StartAsync.");
    }

    [Test]
    public async Task UnsubscribeAsync_WhenConnected_UnsubscribesFromTopics()
    {
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);
        var topics = new List<string> { "test/topic1", "test/topic2" };

        await _managedClient.UnsubscribeAsync(topics, CancellationToken.None);

        _mockMqttClient.Verify(c => c.UnsubscribeAsync(
            It.Is<MqttClientUnsubscribeOptions>(o => o.TopicFilters.Count == 2),
            It.IsAny<CancellationToken>()), Times.Once,
            "UnsubscribeAsync should call internal client's UnsubscribeAsync with correct topic filters when connected.");
    }

    #endregion

    #region Event Tests

    [Test]
    public void ConnectedAsync_WhenSubscribed_ForwardsToInternalClient()
    {
        Func<MqttClientConnectedEventArgs, Task> handler = _ => Task.CompletedTask;
        _managedClient.ConnectedAsync += handler;

        _mockMqttClient.VerifyAdd(c => c.ConnectedAsync += It.IsAny<Func<MqttClientConnectedEventArgs, Task>>(), Times.Once);

        _managedClient.ConnectedAsync -= handler;
        _mockMqttClient.VerifyRemove(c => c.ConnectedAsync -= It.IsAny<Func<MqttClientConnectedEventArgs, Task>>(), Times.Once);
    }

    [Test]
    public void DisconnectedAsync_WhenSubscribed_ForwardsToInternalClient()
    {
        Func<MqttClientDisconnectedEventArgs, Task> handler = _ => Task.CompletedTask;
        _managedClient.DisconnectedAsync += handler;

        _mockMqttClient.VerifyAdd(c => c.DisconnectedAsync += It.IsAny<Func<MqttClientDisconnectedEventArgs, Task>>(), Times.Once);

        _managedClient.DisconnectedAsync -= handler;
        _mockMqttClient.VerifyRemove(c => c.DisconnectedAsync -= It.IsAny<Func<MqttClientDisconnectedEventArgs, Task>>(), Times.Once);
    }

    [Test]
    public void ApplicationMessageReceivedAsync_WhenSubscribed_ForwardsToInternalClient()
    {
        Func<MqttApplicationMessageReceivedEventArgs, Task> handler = _ => Task.CompletedTask;
        _managedClient.ApplicationMessageReceivedAsync += handler;

        _mockMqttClient.VerifyAdd(c => c.ApplicationMessageReceivedAsync += It.IsAny<Func<MqttApplicationMessageReceivedEventArgs, Task>>(), Times.Once);

        _managedClient.ApplicationMessageReceivedAsync -= handler;
        _mockMqttClient.VerifyRemove(c => c.ApplicationMessageReceivedAsync -= It.IsAny<Func<MqttApplicationMessageReceivedEventArgs, Task>>(), Times.Once);
    }

    #endregion

    #region Storage Persistence Tests

    [Test]
    public async Task StopAsync_WithPendingMessages_SavesMessagesToStorage()
    {
        var optionsWithStorage = new MqttClientUserOptions
        {
            MaxPendingMessages = 10,
            PendingMessageStrategy = MqttMessagesOverflowStrategy.RejectNewMessage,
            MessageStorage = _mockStorage.Object
        };
        var managedClientWithStorage = new ManagedMqttClient(
            _mockMqttClient.Object,
            _mockLogger.Object,
            optionsWithStorage);
        var options = new MqttClientOptions();
        var connectResult = new MqttClientConnectResult();
        _mockMqttClient.Setup(c => c.ConnectAsync(It.IsAny<MqttClientOptions>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(connectResult);
        _mockMqttClient.Setup(c => c.IsConnected).Returns(true);
        _mockMqttClient.Setup(c => c.Options).Returns(options);
        _mockMqttClient.Setup(c => c.PublishAsync(It.IsAny<MqttApplicationMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MqttClientPublishResult(null, MqttClientPublishReasonCode.Success, null, new List<MqttUserProperty>()));
        _mockStorage.Setup(s => s.LoadQueuedMessagesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MqttApplicationMessage>());
        _mockStorage.Setup(s => s.SaveQueuedMessagesAsync(
            It.IsAny<IList<MqttApplicationMessage>>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(TimeSpan.FromMilliseconds(100));

        try
        {
            await managedClientWithStorage.StartAsync(options, cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Can be expected when we cancel the token
        }
        // Enqueue a message
        var message = new MqttApplicationMessageBuilder()
            .WithTopic("test/topic")
            .WithPayload([1, 2, 3])
            .Build();
        await managedClientWithStorage.EnqueueAsync(message, CancellationToken.None);
        // Stop should save pending messages to storage
        await managedClientWithStorage.StopAsync(CancellationToken.None);

        _mockStorage.Verify(s => s.SaveQueuedMessagesAsync(
            It.IsAny<IList<MqttApplicationMessage>>(),
            It.IsAny<CancellationToken>()), Times.AtLeastOnce,
            "SaveQueuedMessagesAsync should be called during StopAsync to persist pending messages.");
        managedClientWithStorage.Dispose();
    }

    #endregion

    #region Overflow Strategy Tests

    [Test]
    public async Task EnqueueAsync_QueueFull_DropOldestQueuedMessage_RemovesOldestMessage()
    {
        var pendingMessageCount = 3;
        var optionsWithDropOldest = new MqttClientUserOptions
        {
            MaxPendingMessages = pendingMessageCount,
            PendingMessageStrategy = MqttMessagesOverflowStrategy.DropOldestQueuedMessage
        };
        var managedClientDropOldest = new ManagedMqttClient(
            _mockMqttClient.Object,
            _mockLogger.Object,
            optionsWithDropOldest);
        MqttApplicationMessage? lastSkippedMessage = null;
        managedClientDropOldest.MessageSkipped += (msg) =>
        {
            lastSkippedMessage = msg;
            return Task.CompletedTask;
        };
        var options = new MqttClientOptions();
        _mockMqttClient.Setup(c => c.Options).Returns(options);

        // Fill the queue with 3 messages
        for (var i = 0; i < pendingMessageCount; i++)
        {
            await managedClientDropOldest.EnqueueAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic($"test/topic{i}")
                    .WithPayload([(byte)i])
                    .Build(),
                CancellationToken.None);
        }
        // Add a 4th message; oldest (topic0) should be dropped
        var fourthMessage = new MqttApplicationMessageBuilder()
            .WithTopic("test/topic_new")
            .WithPayload([99])
            .Build();
        await managedClientDropOldest.EnqueueAsync(fourthMessage, CancellationToken.None);

        Assert.That(lastSkippedMessage, Is.Not.Null);
        Assert.That(lastSkippedMessage.Topic, Is.EqualTo("test/topic0"));
        managedClientDropOldest.Dispose();
    }

    #endregion
}
