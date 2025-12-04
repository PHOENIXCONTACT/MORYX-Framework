// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Drivers;
using MQTTnet;
using System.ComponentModel;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;
using Moryx.StateMachines;
using Moryx.Threading;
using MQTTnet.Formatter;
using Microsoft.Extensions.Logging;
using MQTTnet.Protocol;
using Moryx.Configuration;
using System.Security.Claims;
using Moryx.Drivers.Mqtt.Properties;
using System.ComponentModel.DataAnnotations;
using Moryx.Drivers.Mqtt.States;
using System.Diagnostics;
using MQTTnet.Exceptions;

namespace Moryx.Drivers.Mqtt;

/// <summary>
/// Driver, which is able to subscribe several MQTT Topics referenced as Channels
/// </summary>
[ResourceRegistration]
[Display(Name = nameof(Strings.MqttDriver_DisplayName), Description = nameof(Strings.MqttDriver_Description), ResourceType = typeof(Strings))]
public class MqttDriver : Driver, IMessageDriver
{

    private string _clientId;
    /// <inheritdoc/>
    public event EventHandler<object> Received;

    /// <summary>
    /// Timer used in message queue
    /// </summary>
    public IParallelOperations ParallelOperations { get; set; }

    internal static readonly ActivitySource _activitySource = new ActivitySource("Moryx.Drivers.Mqtt.MqttDriver");

    #region EntrySerialize

    /// <summary>
    /// String representation of <see cref="Driver.CurrentState"/>
    /// </summary>
    [EntrySerialize]
    [Display(Name = nameof(Strings.MqttDriver_DriverState), ResourceType = typeof(Strings))]
    public string DriverState => CurrentState?.ToString() ?? string.Empty;

    /// <summary>
    /// Returns this driver object
    /// </summary>
    public IDriver Driver => this;

    /// <summary>
    /// Name of the Root Topic
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue("root/")]
    [Display(Name = nameof(Strings.MqttDriver_Identifier), ResourceType = typeof(Strings))]
    public string Identifier { get => _identifier; set => ConfigChange(ref _identifier, value); }

    /// <summary>
    /// URL or IP-Address of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue("127.0.0.1")]
    [Display(Name = nameof(Strings.MqttDriver_BrokerUrl), Description = nameof(Strings.MqttDriver_BrokerUrl_Description), ResourceType = typeof(Strings))]
    public string BrokerUrl
    {
        get => _brokerUrl;
        set => ConfigChange(ref _brokerUrl, value);
    }

    private void ConfigChange<T>(ref T field, T value) where T : IEquatable<T>
    {
        if (field?.Equals(value) ?? value == null)
        {
            return;
        }
        field = value;
        Reconnect();
    }

    /// <summary>
    /// Function to trigger a reconnect to the broker. Can be necesseray if the configuration was changed or as a debugging tool.
    /// </summary>
    [EntrySerialize]
    [Display(
        Name = nameof(Strings.MqttDriver_Reconnect_Name),
        Description = nameof(Strings.MqttDriver_Reconnect_Description),
        ResourceType = typeof(Strings))]
    public void Reconnect()
    {
        if (State is null || State.Classification != StateClassification.Running)
        {
            return;
        }

        State.Disconnect();
        State.Connect();
    }

    /// <summary>
    /// Port of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue(1883)]
    [Display(Description = nameof(Strings.MqttDriver_Port_Description), ResourceType = typeof(Strings))]
    public int Port { get => _port; set => ConfigChange(ref _port, value); }

    /// <summary>
    /// Port of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.MqttDriver_UseTls), Description = nameof(Strings.MqttDriver_UseTls_Description), ResourceType = typeof(Strings))]
    public bool UseTls { get; set; }

    /// <summary>
    /// Port of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.MqttDriver_Username), Description = nameof(Strings.MqttDriver_Username_Description), ResourceType = typeof(Strings))]
    public string Username { get; set; }

    /// <summary>
    /// Port of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember, Password]
    [Display(Name = nameof(Strings.MqttDriver_Password), Description = nameof(Strings.MqttDriver_Password_Description), ResourceType = typeof(Strings))]
    public string Password { get; set; }

    /// <summary>
    /// Version of the MQTT-Protocol
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue(MqttProtocolVersion.V311)]
    [Display(Name = nameof(Strings.MqttDriver_MqttVersion), Description = nameof(Strings.MqttDriver_MqttVersion_Description), ResourceType = typeof(Strings))]
    public MqttProtocolVersion MqttVersion { get; set; }

    /// <summary>
    /// MQTT Quality of service for sent messages
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue(nameof(MqttQualityOfServiceLevel.ExactlyOnce)), MqttQoS]
    [Display(Name = nameof(Strings.MqttDriver_QualityOfService), Description = nameof(Strings.MqttDriver_QualityOfService_Description), ResourceType = typeof(Strings))]
    public string QualityOfService { get; set; }

    /// <summary>
    /// Configure whether we reconnect with clean session
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(Name = nameof(Strings.MqttDriver_ReconnectWithoutCleanSession), Description = nameof(Strings.MqttDriver_ReconnectWithoutCleanSession_Description), ResourceType = typeof(Strings))]
    public bool ReconnectWithoutCleanSession { get; set; }

    /// <summary>
    /// Configuration to record the entire message content of a received or published to the corresponding System.Diagnostics.Activity
    /// </summary>
    [DataMember, EntrySerialize]
    [Display(
        Name = nameof(Strings.MqttDriver_TraceMessageContent_Name),
        Description = nameof(Strings.MqttDriver_TraceMessageContent_Description),
        ResourceType = typeof(Strings)
    )]
    public bool TraceMessageContent { get; set; }

    /// <summary>
    /// All MQTT Topics the driver subscribed
    /// </summary>
    [ReferenceOverride(nameof(Children))]
    public IReferences<MqttTopic> Channels { get; set; }

    /// <summary>
    /// Delay for opening a new connection after error induced reconnect
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue(100)]
    [Display(Name = nameof(Strings.MqttDriver_ReconnectDelayMs), Description = nameof(Strings.MqttDriver_ReconnectDelayMs_Description), ResourceType = typeof(Strings))]
    public int ReconnectDelayMs { get; set; }

    /// <summary>
    /// Decides if a last will message should be registered on the broker.
    /// A last will message is then automatically published by the broker to
    /// let other clients know that this client was disconnected
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(
        Name = nameof(Strings.MqttDriver_HasLastWill),
        Description = nameof(Strings.MqttDriver_HasLastWill_Description),
        ResourceType = typeof(Strings)
    )]
    public bool HasLastWill { get; set; }

    /// <summary>
    /// The topic the last will message should be published to. Has no effect if HasLastWill is not set
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(
        Name = nameof(Strings.MqttDriver_LastWillTopic),
        Description = nameof(Strings.MqttDriver_LastWillTopic_Description),
        ResourceType = typeof(Strings)
    )]
    public string LastWillTopic { get; set; }

    /// <summary>
    /// Content of the last will message. Has no effect if HasLastWill is not set
    /// </summary>
    [EntrySerialize, DataMember]
    [Display(
        Name = "Last will content",
        Description = "Content of the last will message. Has no effect if HasLastWill is not set",
        ResourceType = typeof(Strings)
    )]
    public string LastWillContent { get; set; }

    #endregion

    #region Properties
    /// <inheritdoc />
    public bool HasChannels => Channels.Count > 0;
    internal DriverMqttState State => (DriverMqttState)CurrentState;
    private IMqttClient _mqttClient;
    private string _brokerUrl;
    private string _identifier;
    private int _port;
    private int _scheduledReconnect;

    #endregion

    #region Lifecycle

    /// <inheritdoc />
    protected override void OnInitialize()
    {
        base.OnInitialize();

        var factory = new MqttClientFactory();
        _mqttClient = factory.CreateMqttClient();

        _mqttClient.ApplicationMessageReceivedAsync += OnReceived;

        //Connection with MQTT-Broker is established
        _mqttClient.ConnectedAsync += OnConnected;

        _mqttClient.DisconnectedAsync += OnDisconnected;

        StateMachine.Initialize(this).With<DriverMqttState>();
    }

    internal void InitializeForTest(IMqttClient client)
    {
        _mqttClient = client;
        _mqttClient.ApplicationMessageReceivedAsync += OnReceived;
        _mqttClient.ConnectedAsync += OnConnected;
        _mqttClient.DisconnectedAsync += OnDisconnected;
        StateMachine.Initialize(this).With<DriverMqttState>();
    }

    /// <inheritdoc />
    protected override void OnStart()
    {
        base.OnStart();

        State.Connect();
    }

    /// <inheritdoc />
    protected override void OnStop()
    {
        State.Disconnect();

        base.OnStop();
    }

    /// <inheritdoc />
    protected override void OnDispose()
    {
        if (_mqttClient != null)
        {
            _mqttClient.ApplicationMessageReceivedAsync -= OnReceived;

            _mqttClient.ConnectedAsync -= OnConnected;

            _mqttClient.DisconnectedAsync -= OnDisconnected;
        }

        base.OnDispose();
    }

    #endregion

    internal async Task Connect(bool firstConnect = true)
    {
        var options = ConfigureMqttClient().Build();
        try
        {
            var result = await _mqttClient.ConnectAsync(
                options, CancellationToken.None);
            if (result.ReasonString != null)
            {
                Logger.Log(LogLevel.Information, "Server returned {reason} on connection attemt", result.ReasonString);
            }
            else
            {
                Logger.Log(LogLevel.Information, "Connection attempt to mqtt broker");
            }
        }
        catch (Exception e)
        {
            // This only throws an exceptions if the server is not available, other cases could still mean we are not connected
            if (firstConnect)
            {
                Logger.Log(LogLevel.Warning, "Error while connecting to Broker: {message}", e.Message);
            }

            State.TriedConnecting(false);
        }
    }

    private MqttClientOptionsBuilder ConfigureMqttClient()
    {
        _clientId = $"{System.Net.Dns.GetHostName()}-{Id}-{Name}";
        var optionsBuilder = new MqttClientOptionsBuilder()
            .WithClientId(_clientId)
            .WithTcpServer(BrokerUrl, Port)
            .WithTlsOptions(new MqttClientTlsOptions() { UseTls = UseTls })
            .WithCleanSession(!ReconnectWithoutCleanSession);

        if (HasLastWill)
        {
            optionsBuilder
                .WithWillRetain(true)
                .WithWillTopic(Identifier + LastWillTopic)
                .WithWillPayload(LastWillContent);
        }

        if (!string.IsNullOrEmpty(Username))
        {
            optionsBuilder.WithCredentials(Username, Password);
        }

        if (MqttVersion != MqttProtocolVersion.Unknown)
        {
            optionsBuilder = optionsBuilder.WithProtocolVersion(MqttVersion);
        }
        else
        {
            optionsBuilder = optionsBuilder.WithProtocolVersion(MqttProtocolVersion.V310);
        }

        return optionsBuilder;
    }

    internal async Task OnConnected(MqttClientConnectedEventArgs args)
    {
        Logger.Log(LogLevel.Information, "Driver {id} connected to MqttBroker", _mqttClient.Options?.ClientId);

        State.TriedConnecting(true);
        var tasks = Channels
            .Where(c => c.TopicType != TopicType.PublishOnly)
            .Select(c => SubscribeTopicAsync(c.SubscribedTopic));
        await Task.WhenAll(tasks);
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        if (args.ClientWasConnected) // Only log info if we were connected before
        {
            Logger.Log(LogLevel.Information, "Driver {id} disconnected from MqttBroker. Reason: {reason}", _mqttClient.Options.ClientId, args.ReasonString);
        }

        State.ConnectionToBrokerLost();

        return Task.CompletedTask;
    }

    internal async Task Disconnect()
    {
        if(_scheduledReconnect != 0)
        {
            ParallelOperations.StopExecution(_scheduledReconnect);
            _scheduledReconnect = 0;
        }

        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions
            {
                Reason = HasLastWill
                    ? MqttClientDisconnectOptionsReason.DisconnectWithWillMessage
                    : MqttClientDisconnectOptionsReason.NormalDisconnection
            });
        }
    }

    /// <inheritdoc />
    public IMessageChannel Channel(string identifier)
    {
        var channelList = Channels.Where(t => t.Identifier.Equals(identifier)).ToList();
        switch (channelList.Count)
        {
            case 0:
                Logger.Log(LogLevel.Warning,
                    "No channels with the name {identifier} exists.", identifier);
                return null;
            case 1:
                return channelList[0];
            default:
                Logger.Log(LogLevel.Warning,
                    "Several channels with the name {identifier} exist.", identifier);
                return null;
        }
    }

    /// <inheritdoc />
    public void Send(object message)
    {
        SendAsync(message).Wait();
    }

    /// <inheritdoc />
    public async Task SendAsync(object message, CancellationToken cancellationToken = default)
    {
        List<MqttTopic> topics;
        try
        {
            // Search by identifier if set
            if (message is IIdentifierMessage identifierMessage
                && !string.IsNullOrEmpty(identifierMessage.Identifier))
            {
                topics = Channels
                    .Where(t =>
                        t.TopicType != TopicType.SubscribeOnly
                        && t.Matches(identifierMessage.Identifier))
                    .ToList();
            }
            else
            {
                topics = Channels
                    .Where(t =>
                        t.TopicType != TopicType.SubscribeOnly
                        && (t.MessageType?.IsInstanceOfType(message) ?? false))
                    .ToList();
            }
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to find matching topic, because of an error");
            throw;
        }

        if (topics.Count == 1)
        {
            await SendInternalAsync(topics[0], message, cancellationToken);
        }
        else
        {
            Logger.Log(LogLevel.Warning, "Corresponding topic for message {message} not found.", message);
        }
    }

    /// <summary>
    /// Send with a topic already preselected.
    /// This is used when a specific topic resource has already been determined.
    /// (Including when Send is called on a Topic Resource directly)
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    internal async Task SendInternalAsync(MqttTopic topic, object message, CancellationToken cancellationToken = default)
    {
        try
        {
            await State.SendAsync(topic, message, cancellationToken);

        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to send message to broker");
            throw;
        }
    }

    /// <summary>
    /// Method to be called if a message should be published on a topic.
    /// </summary>
    /// <param name="messageTopic">The topic to publish on</param>
    /// <param name="message">The message to be published</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task OnSend(MqttMessageTopic messageTopic, byte[] message, CancellationToken cancellationToken)
    {
        using var span = _activitySource.StartActivity("Send",
            ActivityKind.Producer, parentContext: default, tags: new Dictionary<string, object>() {
            {"message.topic", messageTopic.Topic},
            {"message.retain", messageTopic.retain},
            {"message.length", message.Length},
        });

        if (TraceMessageContent)
        {
            span?.AddTag("message.content", System.Text.Encoding.UTF8.GetString(message));
        }

        var messageMqttBuilder = new MqttApplicationMessageBuilder()
            .WithTopic(messageTopic.Topic)
            .WithPayload(message)
            .WithRetainFlag(messageTopic.retain)
            .WithQualityOfServiceLevel(ConvertStringToQoS(QualityOfService));

        if (MqttVersion >= MqttProtocolVersion.V500
            && !string.IsNullOrEmpty(messageTopic.ResponseTopic))
        {
            messageMqttBuilder
                .WithResponseTopic(messageTopic.ResponseTopic)
                .WithUserProperty(ClaimTypes.Sid, _clientId);
        }

        var messageMqtt = messageMqttBuilder.Build();
        await _mqttClient.PublishAsync(messageMqtt, CancellationToken.None);
    }

    private Task OnReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        // Experimental: Dispatch to new thread to prevent exceptions or deadlocks from causing inflight blockage
        ParallelOperations.ExecuteParallel(
            param => Receive(param.ApplicationMessage),
            new { args.ApplicationMessage }
        );

        return Task.CompletedTask;
    }

    internal void Receive(MqttApplicationMessage appMessage)
    {
        var topicName = appMessage.Topic;
        var message = appMessage.Payload;
        using var span = _activitySource.StartActivity("Receive", System.Diagnostics.ActivityKind.Consumer, parentContext: default, tags: new Dictionary<string, object>{
            { "driver.id", Id },
            { "driver.name", Name },
            { "message.topic", topicName },
            { "message.length", message.Length }
        });

        if (TraceMessageContent)
        {
            span?.AddTag("message.content", appMessage.ConvertPayloadToString());
        }

        var topic = topicName;
        if (!string.IsNullOrEmpty(Identifier) && topicName.StartsWith(Identifier))
        {
            topic = topicName.Substring(Identifier.Length);
        }

        var topicList =
            Channels
                .Where(t => t.TopicType != TopicType.PublishOnly && t.Matches(topic))
                .ToList();

        if (topicList.Count >= 1)
        {
            foreach (var topicResource in topicList)
            {
                var responseTopic = MqttVersion switch
                {
                    MqttProtocolVersion.V500 => appMessage.ResponseTopic,
                    _ => null
                };
                topicResource.OnReceived(topic, message, responseTopic, appMessage.Retain);
            }
        }
        else
        {
            Logger.Log(LogLevel.Warning, "Message on topic {topicName} received, but no corresponding Topic Resource found", topicName);
            span?.SetStatus(ActivityStatusCode.Error, "No receiver found");
        }
    }

    /// <summary>
    /// Method to be called when a message for the driver was recieved
    /// </summary>
    /// <param name="messageObject"></param>
    public void OnReceived(object messageObject)
    {
        Received?.Invoke(this, messageObject);
    }

    /// <summary>
    /// Topic is added
    /// </summary>
    /// <param name="topic">Topic to be subscribed</param>
    public void NewTopicAdded(string topic)
    {
        State.AddTopic(topic);
    }

    /// <summary>
    /// This method will be called if a topicName changed. Then the driver
    /// </summary>
    /// <param name="oldTopic"></param>
    /// <param name="newTopic"></param>
    public void OnTopicChanged(string oldTopic, string newTopic)
    {
        var combinedOldTopic = Identifier + oldTopic;
        var result = _mqttClient.UnsubscribeAsync(combinedOldTopic).Result;
        if (MqttVersion == MqttProtocolVersion.V500 && !string.IsNullOrEmpty(result.ReasonString))
        {
            Logger.LogError("Failed to unsubscribe from topic '{topic}': {reason}", combinedOldTopic, result.ReasonString);
        }

        SubscribeTopicAsync(newTopic).Wait();

    }

    internal async Task SubscribeTopicAsync(string topic)
    {
        var topicName = Identifier + topic;
        var topicFilter = new MqttTopicFilterBuilder()
            .WithTopic(topicName)
            .WithQualityOfServiceLevel(ConvertStringToQoS(QualityOfService))
            .Build();

        if (MqttVersion == MqttProtocolVersion.V500)
        {
            topicFilter.NoLocal = true;
        }

        Logger.LogInformation("Subscribing to topic {topic}", topicName);

        var result = await _mqttClient.SubscribeAsync(topicFilter);

        if (MqttVersion == MqttProtocolVersion.V500 && !string.IsNullOrEmpty(result.ReasonString))
        {
            Logger.LogError("Failed to subscribe to topic: {reason}", result.ReasonString);
        }
    }

    private static MqttQualityOfServiceLevel ConvertStringToQoS(string qos)
    {
        if (qos == null || qos.Equals(String.Empty))
        {
            return MqttQualityOfServiceLevel.ExactlyOnce;
        }
        return Enum.Parse<MqttQualityOfServiceLevel>(qos);
    }

    internal void ExistingTopicRemoved(string subscribedTopic)
    {
        try
        {
            if (!_mqttClient.IsConnected)
            {
                return;
            }
            var result = _mqttClient.UnsubscribeAsync(Identifier + subscribedTopic).Result;
        }
        // Ignore exceptions that can occure during shutdown
        catch (ObjectDisposedException)
        {
            // Ignore
        }
        catch (MqttClientDisconnectedException)
        {
            // Ignore
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to unsubscribe from topic {topic}", subscribedTopic);
        }
    }

    internal void DelayedConnectionAttempt()
    {
        _scheduledReconnect = ParallelOperations.ScheduleExecution(async () =>
        {
            // early return, when reconnect was cancelled
            if (_scheduledReconnect == 0)
                return;
            try
            {
                await Connect(false);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Failed to connect to broker");
            }
        }, ReconnectDelayMs, -1);
    }
}

internal class MqttQoSAttribute : PossibleValuesAttribute
{
    /// <inheritdoc />
    public override bool OverridesConversion => false;

    /// <inheritdoc />
    public override bool UpdateFromPredecessor => false;

    public override IEnumerable<string> GetValues(Container.IContainer container, IServiceProvider serviceProvider)
    {
        return Enum.GetNames(typeof(MqttQualityOfServiceLevel));
    }
}

/// <summary>
/// Describes metadata for a message that should be published
/// </summary>
/// <param name="ResponseTopic">MQTT 5 Response topic for the message</param>
/// <param name="Topic">The topic the message should be published on</param>
/// <param name="retain">If the published message should be marked as retain</param>
public record MqttMessageTopic(string ResponseTopic, string Topic, bool retain = false);
