// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using MQTTnet;
using MQTTnet.Packets;

namespace Moryx.AspNetCore.Mqtt;

/// <summary>
/// Provides a managed MQTT client interface that handles automatic reconnections, 
/// message queuing, and connection management for MQTT communication.
/// </summary>
/// <remarks>
/// This interface wraps the underlying MQTT client functionality with additional
/// management features like automatic reconnection handling and message queuing
/// to ensure reliable MQTT communication in distributed systems.
/// </remarks>
public interface IManagedMqttClient : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the client is currently connected to the MQTT broker.
    /// </summary>
    /// <value>
    /// <c>true</c> if the client is connected; otherwise, <c>false</c>.
    /// </value>
    bool IsConnected { get; }

    /// <summary>
    /// Enqueues an application message for publishing to the MQTT broker.
    /// </summary>
    /// <param name="applicationMessage">The MQTT application message to be published.</param>
    /// <returns>A task that represents the asynchronous enqueue operation.</returns>
    /// <remarks>
    /// Messages are queued internally and will be published when a connection is available.
    /// This method does not block if the client is currently disconnected.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="applicationMessage"/> is null.</exception>
    Task EnqueueAsync(MqttApplicationMessage applicationMessage, CancellationToken cancellationToken);

    /// <summary>
    /// Sends a ping packet to the MQTT broker to check connection status.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the ping operation.</param>
    /// <returns>A task that represents the asynchronous ping operation.</returns>
    /// <remarks>
    /// This method can be used to verify that the connection to the broker is still active.
    /// If the ping fails, it may indicate connection issues.
    /// </remarks>
    /// <exception cref="InvalidOperationException">Thrown when the client is not connected.</exception>
    Task PingAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Starts the managed MQTT client with the specified connection options.
    /// </summary>
    /// <param name="options">The MQTT client options for establishing the connection.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>
    /// This method initiates the connection to the MQTT broker and enables
    /// automatic reconnection handling. The client will attempt to maintain
    /// the connection until explicitly stopped.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    Task StartAsync(MqttClientOptions options, CancellationToken cancellationToken);

    /// <summary>
    /// Stops the managed MQTT client and disconnects from the broker.
    /// </summary>
    /// <param name="cleanDisconnect">
    /// <c>true</c> to send a proper disconnect packet before closing the connection;
    /// <c>false</c> to close the connection immediately without notification.
    /// </param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>
    /// When stopped, the client will no longer attempt automatic reconnection.
    /// Any queued messages that haven't been published may be lost unless
    /// persistence is configured.
    /// </remarks>
    Task StopAsync(CancellationToken cancellationToken, bool cleanDisconnect = true);

    /// <summary>  
    /// Subscribes to one or more MQTT topics with the specified topic filters.
    /// </summary>
    /// <param name="topicFilters">A collection of topic filters defining the subscriptions.</param>
    /// <returns>A task that represents the asynchronous subscribe operation.</returns>
    /// <remarks>
    /// Topic filters can include wildcards (+, #) to subscribe to multiple topics.
    /// The subscription will be automatically restored after reconnection.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="topicFilters"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the client is not started.</exception>
    Task SubscribeAsync(IEnumerable<MqttTopicFilter> topicFilters, CancellationToken cancellationToken);

    /// <summary>
    /// Unsubscribes from the specified MQTT topics.
    /// </summary>
    /// <param name="topics">A collection of topic names to unsubscribe from.</param>
    /// <returns>A task that represents the asynchronous unsubscribe operation.</returns>
    /// <remarks>
    /// After unsubscribing, the client will no longer receive messages published
    /// to these topics. The unsubscription persists across reconnection.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="topics"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the client is not started.</exception>
    Task UnsubscribeAsync(IEnumerable<string> topics, CancellationToken cancellationToken);

    /// <summary>
    /// Event that is raised when an application message is skipped due to queue overflow.
    /// </summary>
    event Func<MqttApplicationMessage, Task> MessageSkipped;

    /// <summary>
    /// Event that is raised when an application message is received from the MQTT broker.
    /// </summary>
    /// <remarks>
    /// Subscribe to this event to handle incoming MQTT messages. The event handler
    /// should return a Task to support asynchronous message processing.
    /// </remarks>
    event Func<MqttApplicationMessageReceivedEventArgs, Task> ApplicationMessageReceivedAsync;

    /// <summary>
    /// Event that is raised when the client successfully connects to the MQTT broker.
    /// </summary>
    /// <remarks>
    /// This event is triggered after a successful connection establishment,
    /// including automatic reconnection managed by the client.
    /// </remarks>
    event Func<MqttClientConnectedEventArgs, Task> ConnectedAsync;

    /// <summary>
    /// Event that is raised when the client disconnects from the MQTT broker.
    /// </summary>
    /// <remarks>
    /// This event is triggered for both graceful disconnections and unexpected
    /// connection losses. The managed client will attempt automatic reconnection
    /// unless explicitly stopped.
    /// </remarks>
    event Func<MqttClientDisconnectedEventArgs, Task> DisconnectedAsync;
}