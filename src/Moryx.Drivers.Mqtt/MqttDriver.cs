// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Drivers;
using MQTTnet;
using MQTTnet.Client;
using System.ComponentModel;
using System.Text.RegularExpressions;
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
    public string Identifier { get; set; }

    /// <summary>
    /// URL or IP-Address of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue("127.0.0.1")]
    [Display(Name = nameof(Strings.MqttDriver_BrokerUrl), Description = nameof(Strings.MqttDriver_BrokerUrl_Description), ResourceType = typeof(Strings))]
    public string BrokerUrl { get; set; }

    /// <summary>
    /// Port of the MQTT Broker
    /// </summary>
    [EntrySerialize, DataMember, DefaultValue(1883)]
    [Display(Description = nameof(Strings.MqttDriver_Port_Description), ResourceType = typeof(Strings))]
    public int Port { get; set; }

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

    #endregion

    #region Properties
    /// <inheritdoc />
    public bool HasChannels => Channels.Count > 0;
    internal DriverMqttState State => (DriverMqttState)CurrentState;
    private IMqttClient _mqttClient;

    #endregion

    #region Lifecycle

    /// <inheritdoc />
    protected override void OnInitialize()
    {
        base.OnInitialize();

        var factory = new MqttFactory();
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
            var result = await _mqttClient.ConnectAsync(options, CancellationToken.None);
            if (result.ReasonString != null)
                Logger.Log(LogLevel.Information, "Server returned {reason} on connection attemt", result.ReasonString);
            else
                Logger.Log(LogLevel.Information, "Connection attempt to mqtt broker");

        }
        catch (Exception e)
        {
            // This only throws an exceptions if the server is not available, other cases could still mean we are not connected
            if (firstConnect)
                Logger.Log(LogLevel.Warning, "Error while connecting to Broker: {message}", e.Message);
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

        if (!string.IsNullOrEmpty(Username))
            optionsBuilder.WithCredentials(Username, Password);

        if (MqttVersion != MqttProtocolVersion.Unknown)
            optionsBuilder = optionsBuilder.WithProtocolVersion(MqttVersion);
        else
            optionsBuilder = optionsBuilder.WithProtocolVersion(MqttProtocolVersion.V310);

        return optionsBuilder;
    }

    internal async Task OnConnected(MqttClientConnectedEventArgs args)
    {
        Logger.Log(LogLevel.Information, "Driver {id} connected to MqttBroker", _mqttClient.Options?.ClientId);

        State.TriedConnecting(true);
        var tasks = Channels.Select(c => SubscribeTopicAsync(c.SubscribedTopic));
        await Task.WhenAll(tasks);
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs args)
    {
        if (args.ClientWasConnected) // Only log info if we were connected before
            Logger.Log(LogLevel.Information, "Driver {id} disconnected from MqttBroker. Reason: {reason}", _mqttClient.Options.ClientId, args.ReasonString);

        State.ConnectionToBrokerLost();

        return Task.CompletedTask;
    }

    internal async Task Disconnect()
    {
        if (_mqttClient.IsConnected)
        {
            await _mqttClient.DisconnectAsync(new MqttClientDisconnectOptions { Reason = MqttClientDisconnectOptionsReason.NormalDisconnection });
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
        IReadOnlyList<MqttTopic> topics;
        // Search by identifier if set
        if (message is IIdentifierMessage identifierMessage && !string.IsNullOrEmpty(identifierMessage.Identifier))
            topics = Channels.Where(t => t.Matches(identifierMessage.Identifier)).ToList();
        else
            topics = Channels.Where(t => t.MessageType.IsInstanceOfType(message)).ToList();

        if (topics.Count == 1)
            await State.SendAsync(topics[0], message, cancellationToken);
        else
            Logger.Log(LogLevel.Warning, "Corresponding topic for message {message} not found.", message);
    }

    /// <summary>
    /// Method to be called if a message should be published on a topic.
    /// </summary>
    /// <param name="messageTopic">The topic to publish on</param>
    /// <param name="message">The message to be published</param>
    /// <returns></returns>
    public async Task OnSend(MqttMessageTopic messageTopic, byte[] message)
    {
        var messageMqttBuilder = new MqttApplicationMessageBuilder()
          .WithTopic(messageTopic.Topic)
          .WithPayload(message)
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
        ParallelOperations.ExecuteParallel(param => Receive(param.Topic, param.Payload), new { args.ApplicationMessage.Topic, args.ApplicationMessage.Payload });

        return Task.CompletedTask;
    }

    internal void Receive(string topicName, byte[] message)
    {

        var topic = topicName;
        if (Identifier != "")
        {
            var regex = new Regex(Regex.Escape(Identifier));
            topic = regex.Replace(topicName, "", 1);
        }
        var topicList = Channels.Where(t => t.Matches(topic)).ToList();
        if (topicList.Count >= 1)
        {
            foreach (var topicResource in topicList)
                topicResource.OnReceived(topic, message);
        }
        else
        {
            Logger.Log(LogLevel.Warning, "Message on topic {topicName} received, but no corresponding Topicresource found", topicName);
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
        var result = _mqttClient.UnsubscribeAsync(Identifier + oldTopic).Result;
        if (MqttVersion == MqttProtocolVersion.V500 && !string.IsNullOrEmpty(result.ReasonString))
            Logger.LogError("Failed to unsubscribe from topic: {reason}", result.ReasonString);
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
            topicFilter.NoLocal = true;
        var result = await _mqttClient.SubscribeAsync(topicFilter);

        if (MqttVersion == MqttProtocolVersion.V500 && !string.IsNullOrEmpty(result.ReasonString))
            Logger.LogError("Failed to subscribe to topic: {reason}", result.ReasonString);
    }

    private static MqttQualityOfServiceLevel ConvertStringToQoS(string qos)
    {
        if (qos == null || qos.Equals(String.Empty))
            return MqttQualityOfServiceLevel.ExactlyOnce;
        return (MqttQualityOfServiceLevel)Enum.Parse(typeof(MqttQualityOfServiceLevel), qos);
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

public record MqttMessageTopic(string ResponseTopic, string Topic);

