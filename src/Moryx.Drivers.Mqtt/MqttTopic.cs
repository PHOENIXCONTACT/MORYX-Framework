// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.Bindings;
using Moryx.Serialization;
using Moryx.Threading;
using Moryx.Tools;
using Moryx.Drivers.Mqtt.Properties;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Moryx.Drivers.Mqtt.Messages;
using System.Buffers;
using System.Text.Json;

namespace Moryx.Drivers.Mqtt
{

    /// <summary>
    /// Base class for MQTT topics a MQTT Driver can subscribe.
    /// </summary>
    [ResourceRegistration]
    public abstract class MqttTopic : Resource, IMessageChannel
    {
        /// <summary>
        /// Injected service, used for scheduling tasks
        /// </summary>
        public IParallelOperations ParallelOperations { get; set; }

        #region Properties
        /// <summary>
        /// Name of the MQTT Topic
        /// </summary>
        [DataMember, EntrySerialize]
        [Display(Name = nameof(Strings.MqttTopic_Identifier), Description = nameof(Strings.MqttTopic_Identifier_Description), ResourceType = typeof(Strings))]
        public string Identifier
        {
            get => _topicName;
            set
            {
                var (validationResult, errorMessage) = ValidateTopicString(value);

                switch (validationResult)
                {
                    case TopicValidationResult.Valid:
                        //replace +
                        var pattern = value.Replace("+", @"\w+");
                        //replace #
                        pattern = pattern.Replace("#", @"\w+(?:/\w+)*");
                        //replace placeholder
                        pattern = value.Contains('{') ?
                            pattern.Replace("{", "(?<").Replace("}", @">(?:\w|\:)+)") : pattern;
                        pattern = pattern.Replace(".", "__");
                        pattern += "$";
                        RegexTopic = new Regex(pattern);

                        var regex = new Regex(@"({\w*})");
                        _subscribedTopic = regex.Replace(value.Replace(".", "__"), "+");

                        var oldTopic = _topicName;
                        _topicName = value;
                        if (oldTopic != null)
                        {
                            ParallelOperations.ExecuteParallel(ReportToDriverThatTopicChanged,
                                new TopicChanged(oldTopic, SubscribedTopic));
                        }
                        break;
                    case TopicValidationResult.Invalid:
                        // TODO: Discuss if failing silently is a good idea
                        Logger.LogWarning("Invalid Identifier specified. Not overwriting the _topicName. Reason: {errorMessage}", errorMessage);
                        break;
                    case TopicValidationResult.Uninitialized:
                        // Ignore overwriting null with null
                        break;
                }
            }
        }

        /// <summary>
        /// Determines if the topic can be written to, read from or both
        /// </summary>
        [DataMember, EntrySerialize]
        [Display(Name = nameof(Strings.MqttTopic_TopicType), Description = nameof(Strings.MqttTopic_TopicType_Description), ResourceType = typeof(Strings))]
        public TopicType TopicType { get; set; }

        /// <summary>
        /// "Determines if the decoded message is stored as part of the diagnostics span for debugging purposes
        /// </summary>
        [DataMember, EntrySerialize]
        [Display(Name = nameof(Strings.MqttTopic_TraceDecodedMessage), Description = nameof(Strings.MqttTopic_TraceDecodedMessage_Description), ResourceType = typeof(Strings))]
        public bool TraceDecodedMessage { get; set; }

        private (TopicValidationResult result, string errorMessage) ValidateTopicString(string value)
        {
            if (value is null)
            {
                if (_topicName is null)
                {
                    return (TopicValidationResult.Uninitialized, "topic is null");
                }

                return (TopicValidationResult.Invalid, "topic is null"); ;
            }

            var regexPlaceholders = new Regex(@"(\w|})({\w*})|({\w*})(\w|{)");
            var regexWildcards = new Regex(@"(([^/](\+|#)|(\+|#)[^/]))");
            if (regexPlaceholders.IsMatch(value))
            {
                var logMessage = $"Topic {value} does not match requirements.Things like abv{{foo}} and {{foo}}{{foo2}} are not allowed.";
                return (TopicValidationResult.Invalid, logMessage);
            }
            else if (regexWildcards.IsMatch(value))
            {
                var logMessage = $"Topic {value} does not match requirements. Things like abv+, abc#, +as, #sdf and +# are not allowed.";
                return (TopicValidationResult.Invalid, logMessage);
            }
            else if (value.Contains(' '))
            {
                return (TopicValidationResult.Invalid, "Topic should not contain spaces.");
            }
            return (TopicValidationResult.Valid, null);

        }

        /// <summary>
        /// Topic that other clients may use to respond to this message.
        /// <remarks>Only available for MQTT v5+</remarks>
        /// </summary>
        [EntrySerialize, DataMember, DefaultValue("")]
        [Display(Name = nameof(Strings.MqttTopic_ResponseTopic), Description = nameof(Strings.MqttTopic_ResponseTopic_Description), ResourceType = typeof(Strings))]
        public string ResponseTopic { get; set; }

        /// <summary>
        /// Marks if messages send by this topic should be marked as retained by default.
        /// Messages can implment IRetainAware to overwrite the default.
        /// </summary>
        [EntrySerialize, DataMember]
        [Display(Name = nameof(Strings.MqttTopic_Retain), Description = nameof(Strings.MqttTopic_Retain_Description), ResourceType = typeof(Strings))]
        public bool Retain { get; set; }

        private void ReportToDriverThatTopicChanged(TopicChanged args)
        {
            if (TopicType == TopicType.PublishOnly) // TODO check for changes in topic type too
            {
                return;
            }

            MqttDriver.OnTopicChanged(args.OldTopic, args.NewTopic);
        }

        private class TopicChanged
        {
            public readonly string OldTopic;
            public readonly string NewTopic;

            public TopicChanged(string oldTopic, string newTopic)
            {
                OldTopic = oldTopic;
                NewTopic = newTopic;
            }
        }

        private string _topicName;
        /// <summary>
        /// cached regex of the topic identifier
        /// </summary>
        protected internal Regex RegexTopic;

        /// <summary>
        /// corresponding topic to be subscribed
        /// placeholders are replaced with +
        /// </summary>
        [EntrySerialize]
        public string SubscribedTopic => _subscribedTopic;

        private string _subscribedTopic;

        /// <summary>
        /// Name of the type sent through this topic
        /// </summary>
        public abstract string MessageName { get; set; }

        /// <summary>
        /// Type sent through this topic
        /// </summary>
        public Type MessageType { get; set; }

        /// <summary>
        /// Output the type name of the resolved MessageType.
        /// This allows the user to check if the Type resolution has succeeded.
        /// </summary>
        [EntrySerialize]
        public string ResolveTypeName => MessageType?.FullName ?? "Unresolved";

        /// <summary>
        /// Driver, who subscribes this topic.
        /// </summary>
        public IDriver Driver => MqttDriver;

        /// <summary>
        /// Typed reference to the parent
        /// </summary>
        protected MqttDriver MqttDriver => (MqttDriver)Parent;

        #endregion

        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <inheritdoc />
        protected override async Task OnStartAsync(CancellationToken cancellationToken)
        {
            await base.OnStartAsync(cancellationToken);
            MqttDriver?.NewTopicAdded(SubscribedTopic);
        }

        /// <param name="cancellationToken">The token to monitor for cancellation requests. The default value is None.</param>
        /// <inheritdoc />
        protected override Task OnStopAsync(CancellationToken cancellationToken)
        {
            MqttDriver?.ExistingTopicRemoved(SubscribedTopic);
            return base.OnStopAsync(cancellationToken);
        }

        /// <inheritdoc />
        public abstract void Send(object payload);

        /// <inheritdoc />
        public abstract Task SendAsync(object payload, CancellationToken cancellationToken = default);

        //This method has to call MqttDriver.OnSend
        internal abstract Task OnSend(object payload, CancellationToken cancellationToken);

        //This method has to call MqttDriver.OnReceive
        internal abstract void OnReceived(string receivedTopic, ReadOnlySequence<byte> messageAsBytes, string responseTopic = null, bool retain = false);

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg"></param>
        protected void RaiseReceived(object msg)
        {
            Received?.Invoke(this, msg);
        }

        /// <summary>
        /// Returns True if given Topic matches the topic of this Resource
        /// </summary>
        /// <param name="receivedTopic">topic to be checked</param>
        /// <returns>true, if both topics match</returns>
        public bool Matches(string receivedTopic)
        {
            return RegexTopic.IsMatch(receivedTopic);
        }

        /// <summary>
        /// Event raised, when a message is received
        /// </summary>
        public event EventHandler<object> Received;
    }

    /// <inheritdoc cref="MqttTopic" />
    public abstract class MqttTopic<TMessage> : MqttTopic, IMessageChannel
    {
        private static readonly ActivitySource _activitySource = new ActivitySource("Moryx.Drivers.Mqtt.MqttTopic");

        /// <summary>
        /// Activity source used for tracing messages handled by this topic. Can be overwritten by other implementations
        /// </summary>
        protected virtual ActivitySource ActivitySource => _activitySource;

        /// <summary>
        /// Constructor for MessageType
        /// </summary>
        protected Func<TMessage> Constructor;

        /// <inheritdoc />
        public override string MessageName
        {
            get => MessageType?.Name;
            set
            {
                var typeList = ReflectionTool.GetPublicClasses<TMessage>(m => m.Name == value);
                if (typeList.Length == 1)
                {
                    var type = typeList.First();
                    if (Identifier != default && RegexTopic.GetGroupNames().Length > 1)
                    {
                        var placeholderNames = RegexTopic.GetGroupNames();
                        for (var i = 1; i < placeholderNames.Length; i++)
                        {
                            var placeholder = placeholderNames[i];
                            var prop = type.GetProperty(placeholder);
                            if (prop == null)
                            {
                                Logger.Log(LogLevel.Information, "MessageType {typeName} does not contain a property with the name {placeholder}", type.Name, placeholder);
                                return;
                            }
                        }
                    }
                    MessageType = type;
                    Constructor = ReflectionTool.ConstructorDelegate<TMessage>(MessageType);
                }
                else
                {
                    var type = Type.GetType(value ?? string.Empty);
                    MessageType = type;
                    if (type == null)
                    {
                        return;
                    }

                    Constructor = () => (TMessage)Activator.CreateInstance(MessageType);
                }
            }
        }

        /// <inheritdoc />
        public override void Send(object payload)
        {
            SendAsync(payload).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public override async Task SendAsync(object payload, CancellationToken cancellationToken = default)
        {
            if (payload is not TMessage)
            {
                Logger.Log(LogLevel.Error, "Message {0} has the wrong Type. It is {1} instead of {2}",
                    payload, payload.GetType(), typeof(TMessage));
                throw new ArgumentException("Message " + payload + " has the wrong Type. It is " + payload.GetType() + " instead of " + typeof(TMessage));
            }

            await MqttDriver.SendInternalAsync(this, payload, cancellationToken);
        }

        internal override Task OnSend(object payload, CancellationToken cancellationToken)
        {
            var topic = Identifier;
            var msg = Serialize(payload);
            if (payload is IIdentifierMessage identifierMessage && !string.IsNullOrEmpty(identifierMessage.Identifier))
            {
                topic = identifierMessage.Identifier;
                if (topic.Contains('{'))
                {
                    throw new ArgumentException("Topic " + topic + " of a IdentifierMessage contains placeholders");
                }

                if (!Matches(topic))
                {
                    throw new ArgumentException("Topic " + topic + " of the IdentifierMessage" +
                                                " does not match the topic defined in the Resource");
                }
            }
            else
            {
                var resolver = TextBindingResolverFactory.Create(topic, new BindingResolverFactory());
                topic = resolver.Resolve(payload);
            }
            if (topic.Contains('#') || topic.Contains('+'))
            {
                throw new ArgumentException("Topic to be published on contains wildcards: " + topic);
            }

            if (topic.Contains('{'))
            {
                throw new ArgumentException(
                                   "MessageType does not contain properties matching the placeholder ");
            }

            topic = MqttDriver.Identifier + topic;
            var retain = payload is IRetainAwareMessage ram && ram.Retain.HasValue
                ? ram.Retain.Value
                : Retain;
            return MqttDriver.OnSendAsync(
                new MqttMessageTopic(ResponseTopic, topic, retain),
                msg, cancellationToken);
        }

        internal override void OnReceived(string receivedTopic, ReadOnlySequence<byte> messageAsBytes, string responseTopic, bool retain)
        {
            TMessage msg;
            using (var span = ActivitySource.StartActivity("parsing", ActivityKind.Internal, parentContext: default, tags: new Dictionary<string, object>{
                { "topic.name", Name },
                { "topic.id", Id },
                { "topic.type", MessageName }
            }))
            {
                if (MessageType == null)
                {
                    Logger.Log(LogLevel.Error, "Message was received, but no MessageType was set.");
                    span.SetStatus(ActivityStatusCode.Error, "MessageType not set");
                    return;
                }
                try
                {
                    msg = Deserialize(messageAsBytes);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to deserialize message");
                    span?.AddException(ex);
                    return;
                }

                if (msg is IIdentifierMessage identMessage)
                {
                    identMessage.Identifier = receivedTopic;
                }
                if (responseTopic is not null && msg is IRespondableMessage respondable)
                {
                    respondable.ResponseIdentifier = responseTopic;
                }
                if (msg is IRetainAwareMessage retainAware)
                {
                    retainAware.Retain = retain;
                }

                var groupNames = RegexTopic.GetGroupNames();
                if (groupNames.Length > 1)
                {
                    var placeholderValues = RegexTopic.Match(receivedTopic).Groups;
                    if (placeholderValues.Count == groupNames.Length)
                    {
                        for (var i = 1; i < placeholderValues.Count; i++)
                        {
                            var placeholderName = groupNames[i].Replace("__", ".");
                            var resolver = new BindingResolverFactory().Create(placeholderName);
                            var placeholderValue = placeholderValues[i].ToString();
                            if (!resolver.Update(msg, placeholderValue))
                            {
                                Logger.Log(LogLevel.Error, "Placeholder {placeholderName} cannot be filled. MessageType {typeName} may not contain a matching property", placeholderName, MessageType.Name);
                            }
                        }
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warning, "Number of found Placeholders does not match of number of possible corresponding values");
                    }

                    if (TraceDecodedMessage && span is not null)
                    {
                        span.AddTag("message.decoded", JsonSerializer.Serialize(msg));
                    }
                }
            }

            using (var span = ActivitySource.StartActivity("PublishReceived",
                ActivityKind.Internal, parentContext: default, tags: new Dictionary<string, object>{
                { "topic.name", Name },
                { "topic.id", Id },
                { "topic.type", MessageName }
            }))
            {
                RaiseReceived(msg);
                MqttDriver.OnReceived(msg);
            }
        }

        /// <summary>
        /// Serializes object to a byte-array
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected abstract byte[] Serialize(object payload);

        /// <summary>
        /// deserializes received byte-array to an object of TMessage
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected abstract TMessage Deserialize(ReadOnlySequence<byte> payload);
    }
}
