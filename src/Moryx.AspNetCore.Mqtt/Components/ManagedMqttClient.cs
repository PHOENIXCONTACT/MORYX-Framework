// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Internal;
using MQTTnet.Packets;
using MQTTnet.Protocol;

namespace Moryx.AspNetCore.Mqtt.Components;

/// <summary>
/// Provides a managed MQTT client that handles automatic reconnections, 
/// message queuing, and connection management for MQTT communication.
/// </summary>
internal sealed class ManagedMqttClient : IManagedMqttClient
{
    readonly Queue<MqttApplicationMessage> _messageQueue = new();
    readonly AsyncLock _messageQueueLock = new();
    public bool IsConnected => _internalClient.IsConnected;
    private readonly MqttNetSourceLogger _logger;
    private readonly IMqttClient _internalClient;
    private readonly MqttClientUserOptions _options;
    private readonly ManagedMqttClientStorageManager? _storageManager;
    private MqttApplicationMessage? _removedMessage;
    private Task? _publishingTask;
    private readonly HashSet<MqttTopicFilter> _subscriptions = new();
    private readonly SemaphoreSlim _subscriptionLock = new(1, 1);
    public event Func<MqttApplicationMessage, Task>? MessageSkipped;
    private MqttClientConnectedEventArgs? _lastConnectEventArgs;

    public event Func<MqttApplicationMessageReceivedEventArgs, Task> ApplicationMessageReceivedAsync
    {
        add => _internalClient.ApplicationMessageReceivedAsync += value;
        remove => _internalClient.ApplicationMessageReceivedAsync -= value;
    }

    public event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync
    {
        add
        {
            _internalClient.ConnectedAsync += value;
            // missed event invocation for already connected client
            if (_lastConnectEventArgs != null)
            {
                value.Invoke(_lastConnectEventArgs);
            }
        }
        remove => _internalClient.ConnectedAsync -= value;
    }

    public event Func<MqttClientDisconnectedEventArgs, Task> DisconnectedAsync
    {
        add => _internalClient.DisconnectedAsync += value;
        remove => _internalClient.DisconnectedAsync -= value;
    }

    public ManagedMqttClient(IMqttClient mqttClient, IMqttNetLogger logger, MqttClientUserOptions options)
    {
        _internalClient = mqttClient ?? throw new ArgumentNullException(nameof(mqttClient));
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger.WithSource(nameof(ManagedMqttClient));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        if (options.MessageStorage != null)
        {
            _storageManager = new ManagedMqttClientStorageManager(options.MessageStorage);
        }
    }

    public void Dispose()
    {
        _subscriptionLock.Dispose();
        _messageQueueLock.Dispose();
        _internalClient.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task EnqueueAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(applicationMessage);
        ThrowIfDisposed();

        if (_internalClient.Options == null)
        {
            throw new InvalidOperationException("call StartAsync before publishing messages");
        }

        MqttTopicValidator.ThrowIfInvalid(applicationMessage.Topic);
        MqttApplicationMessage? skipMessage = null;
        try
        {
            using (await _messageQueueLock.EnterAsync(cancellationToken))
            {
                if (_messageQueue.Count >= _options.MaxPendingMessages)
                {
                    if (_options.PendingMessageStrategy == MqttMessagesOverflowStrategy.RejectNewMessage)
                    {
                        skipMessage = applicationMessage;
                        _logger.Verbose("Internal queue is full, skipping publish of new message.");
                        return;
                    }

                    if (_options.PendingMessageStrategy == MqttMessagesOverflowStrategy.DropOldestQueuedMessage)
                    {
                        _removedMessage = _messageQueue.Dequeue();
                        _logger.Verbose("Internal queue is full, removed oldest message from internal queue.");
                        skipMessage = _removedMessage;
                    }
                }

                _messageQueue.Enqueue(applicationMessage);

                if (_storageManager != null)
                {
                    if (_removedMessage != null)
                    {
                        await _storageManager.RemoveAsync(_removedMessage, cancellationToken);
                    }

                    await _storageManager.AddAsync(applicationMessage, cancellationToken);
                }
            }
        }
        finally
        {
            if (skipMessage != null && MessageSkipped != null)
            {
                await MessageSkipped.Invoke(skipMessage);
            }
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_messageQueueLock is null, nameof(ManagedMqttClient));
    }

    public async Task PingAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (!_internalClient.IsConnected)
        {
            throw new InvalidOperationException("Client is not connected.");
        }

        await _internalClient.PingAsync(cancellationToken);
    }

    public async Task StartAsync(MqttClientOptions options, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(options);

        // Load queued messages from storage
        if (_storageManager != null)
        {
            var storedMessages = await _storageManager.LoadQueuedMessagesAsync(cancellationToken);
            if (storedMessages != null && storedMessages.Count > 0)
            {
                foreach (var message in storedMessages)
                {
                    _messageQueue.Enqueue(message);
                }
            }
        }

        // Connect to broker
        var connectionResult = await _internalClient.ConnectAsync(options, cancellationToken);
        _lastConnectEventArgs = new MqttClientConnectedEventArgs(connectionResult);

        // Start the publishing task
        _publishingTask = Task.Run(() => PublishQueuedMessagesAsync(cancellationToken), cancellationToken);

        _logger.Verbose("Managed MQTT client started.");
    }

    public async Task StopAsync(CancellationToken cancellationToken, bool cleanDisconnect = true)
    {
        ThrowIfDisposed();

        try
        {
            // Wait for publishing task to complete
            if (_publishingTask != null)
            {
                await _publishingTask;
            }

            await StorePendingMessagesAsync(cancellationToken);
            await DisconnectAsync(cleanDisconnect);

            _logger.Verbose("Managed MQTT client stopped.");
        }
        finally
        {
            _publishingTask = null;
        }
    }

    private async Task DisconnectAsync(bool cleanDisconnect)
    {
        if (_internalClient.IsConnected)
        {
            var disconnectOptions = new MqttClientDisconnectOptions
            {
                Reason = cleanDisconnect
                    ? MqttClientDisconnectOptionsReason.NormalDisconnection
                    : MqttClientDisconnectOptionsReason.UnspecifiedError
            };
            await _internalClient.DisconnectAsync(disconnectOptions, CancellationToken.None);
            _lastConnectEventArgs = null;
        }
    }

    private async Task StorePendingMessagesAsync(CancellationToken cancellationToken)
    {
        if (_storageManager != null)
        {
            using (await _messageQueueLock.EnterAsync(cancellationToken))
            {
                var pendingMessages = _messageQueue.ToList();
                await _storageManager.SaveQueuedMessagesAsync(pendingMessages, cancellationToken);
                _messageQueue.Clear();
            }
        }
    }

    public async Task SubscribeAsync(IEnumerable<MqttTopicFilter> topicFilters, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(topicFilters);

        if (_internalClient.Options == null)
        {
            throw new InvalidOperationException("Client is not started. Call StartAsync first.");
        }

        var filterList = topicFilters.ToList();
        if (filterList.Count == 0)
        {
            return;
        }

        await _subscriptionLock.WaitAsync(cancellationToken);
        try
        {
            // Add to subscription tracking
            foreach (var filter in filterList)
            {
                _subscriptions.Add(filter);
            }

            // Only subscribe if connected
            if (_internalClient.IsConnected)
            {
                var subscribeOptions = new MqttClientSubscribeOptions
                {
                    TopicFilters = filterList
                };
                await _internalClient.SubscribeAsync(subscribeOptions, cancellationToken);

                _logger.Verbose($"Subscribed to {filterList.Count} topic(s).");
            }
            else
            {
                _logger.Verbose($"Client not connected. {filterList.Count} subscription(s) will be applied on reconnect.");
            }
        }
        finally
        {
            _subscriptionLock.Release();
        }
    }

    public async Task UnsubscribeAsync(IEnumerable<string> topics, CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        ArgumentNullException.ThrowIfNull(topics);

        if (_internalClient.Options == null)
        {
            throw new InvalidOperationException("Client is not started. Call StartAsync first.");
        }

        var topicList = topics.ToList();
        if (topicList.Count == 0)
        {
            return;
        }

        await _subscriptionLock.WaitAsync(cancellationToken);
        try
        {
            // Remove from subscription tracking
            _subscriptions.RemoveWhere(f => topicList.Contains(f.Topic));

            // Only unsubscribe if connected
            if (_internalClient.IsConnected)
            {
                var unsubscribeOptions = new MqttClientUnsubscribeOptions
                {
                    TopicFilters = topicList
                };
                await _internalClient.UnsubscribeAsync(unsubscribeOptions, cancellationToken);

                _logger.Verbose($"Unsubscribed from {topicList.Count} topic(s).");
            }
            else
            {
                _logger.Verbose($"Client not connected. Removed {topicList.Count} subscription(s) from tracking.");
            }
        }
        finally
        {
            _subscriptionLock.Release();
        }
    }

    private async Task PublishQueuedMessagesAsync(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await StartPublishingAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.Verbose("Publishing task canceled.");
        }
    }

    private async Task StartPublishingAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Wait for connection
            while (!_internalClient.IsConnected && !cancellationToken.IsCancellationRequested)
            {
                await WaitAsync(100, cancellationToken);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Resubscribe to topics after reconnection
            await ResubscribeAsync(cancellationToken);

            // Process queued messages
            while (_internalClient.IsConnected && !cancellationToken.IsCancellationRequested)
            {
                await ProcessQueuedMessagesAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Expected when cancellation is requested
            return;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error in publishing task.");
            await WaitAsync(1000, cancellationToken);
        }

    }

    private async Task ProcessQueuedMessagesAsync(CancellationToken cancellationToken)
    {
        MqttApplicationMessage? message = null;
        using (await _messageQueueLock.EnterAsync(cancellationToken))
        {
            if (_messageQueue.Count > 0)
            {
                message = _messageQueue.Peek();
            }
        }

        if (message != null)
        {
            try
            {
                await _internalClient.PublishAsync(message, cancellationToken);

                using (await _messageQueueLock.EnterAsync(cancellationToken))
                {
                    if (_messageQueue.Count > 0 && _messageQueue.Peek() == message)
                    {
                        _messageQueue.Dequeue();
                    }
                }

                if (_storageManager != null)
                {
                    await _storageManager.RemoveAsync(message, cancellationToken);
                }

                _logger.Verbose($"Published message to topic: {message.Topic}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error publishing message to topic: {message.Topic}");
                // If publish fails, we'll retry on next iteration
                await WaitAsync(1000, cancellationToken);
            }
        }
        else
        {
            // No messages in queue, wait a bit
            await WaitAsync(1000, cancellationToken);
        }
    }

    private static async Task WaitAsync(int millisecondsDelay, CancellationToken cancellationToken)
    {
        await Task.Delay(millisecondsDelay, cancellationToken);
    }

    private async Task ResubscribeAsync(CancellationToken cancellationToken)
    {
        await _subscriptionLock.WaitAsync(cancellationToken);
        try
        {
            if (_subscriptions.Count > 0)
            {
                var subscribeOptions = new MqttClientSubscribeOptions
                {
                    TopicFilters = [.. _subscriptions]
                };
                await _internalClient.SubscribeAsync(subscribeOptions, cancellationToken);
                _logger.Verbose($"Resubscribed to {_subscriptions.Count} topic(s) after reconnection.");
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error resubscribing to topics.");
        }
        finally
        {
            _subscriptionLock.Release();
        }
    }
}