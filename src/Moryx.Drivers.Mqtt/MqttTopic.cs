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

namespace Moryx.Drivers.Mqtt
{
    /// <summary>
    /// Base class for MQTT topics a MQTT Driver can subscribe.
    /// </summary>
    [ResourceRegistration]
    public abstract class MqttTopic : Resource, IMessageChannel
    {
        /// <summary>
        /// Event raised, when a message is received
        /// </summary>
        public event EventHandler<object> Received;

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

        private void ReportToDriverThatTopicChanged(TopicChanged args)
        {
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
        protected internal Regex RegexTopic;

        /// <summary>
        /// corresponding topic to be subscribed
        /// placeholders are replaced with +
        /// </summary>
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
        /// Driver, who subscribes this topic.
        /// </summary>
        public IDriver Driver => MqttDriver;
        protected MqttDriver MqttDriver => (MqttDriver)Parent;

        #endregion

        /// <inheritdoc />
        protected override void OnStart()
        {
            base.OnStart();
            MqttDriver?.NewTopicAdded(SubscribedTopic);
        }

        /// <inheritdoc />
        public abstract void Send(object payload);

        /// <inheritdoc />
        public abstract Task SendAsync(object payload, CancellationToken cancellationToken = default);

        //This method has to call MqttDriver.OnSend
        internal abstract Task OnSend(object payload, CancellationToken cancellationToken);

        //This method has to call MqttDriver.OnReceive
        internal abstract void OnReceived(string receivedTopic, byte[] messageAsBytes);

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
        /// log-function, which adjusts logMessages so that they can be logged without format exceptions
        /// </summary>
        /// <param name="level"></param>
        /// <param name="logMessage"></param>
        protected void Log(LogLevel level, string logMessage)
        {
            logMessage = logMessage.Replace("{", @"{{").Replace("}", @"}}");
            Logger.Log(level, logMessage);
        }
    }

    /// <inheritdoc cref="MqttTopic" />
    public abstract class MqttTopic<TMessage> : MqttTopic, IMessageChannel
    {
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
                                Log(LogLevel.Information, "MessageType " + type.Name + " does not contain" +
                                                   " a property with the name " + placeholder);
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
            if (payload is TMessage send)
            {
                MqttDriver.Send(payload);;
            }
            else
            {
                Logger.Log(LogLevel.Error, "Message {0} has the wrong Type. It is {1} instead of {2}",
                    payload, payload.GetType(), typeof(TMessage));
                throw new ArgumentException("Message " + payload + " has the wrong Type. It is " + payload.GetType() + " instead of " + typeof(TMessage));
            }
        }

        /// <inheritdoc />
        public override Task SendAsync(object payload, CancellationToken cancellationToken = default)
        {
            if (payload is TMessage send)
            {
                return MqttDriver.SendAsync(payload, cancellationToken);
            }

            Logger.Log(LogLevel.Error, "Message {0} has the wrong Type. It is {1} instead of {2}",
                payload, payload.GetType(), typeof(TMessage));
            throw new ArgumentException("Message " + payload + " has the wrong Type. It is " + payload.GetType() + " instead of " + typeof(TMessage));
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
            return MqttDriver.OnSend(new MqttMessageTopic(ResponseTopic, topic), msg, cancellationToken);
        }

        internal override void OnReceived(string receivedTopic, byte[] messageAsBytes)
        {
            if (MessageType != null)
            {
                var msg = Deserialize(messageAsBytes);
                if (msg is IIdentifierMessage identMessage)
                {
                    identMessage.Identifier = receivedTopic;
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
                                Logger.Log(LogLevel.Error, "Placeholder " + placeholderName + " cannot be filled. " +
                                                           "MessageType " + typeof(TMessage).Name + " may not contain a " +
                                                           "matching property");
                            }
                        }
                    }
                    else
                    {
                        Logger.Log(LogLevel.Warning, "Number of found Placeholders does not match of number of possible " +
                                                    "corresponding values");
                    }
                }
                RaiseReceived(msg);
                MqttDriver.OnReceived(msg);
            }
            else
            {
                Logger.Log(LogLevel.Error, "Message was received, but not MessageType was set.");
            }
        }

        /// <summary>
        /// Serializes object to a byte-array
        /// </summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        protected internal abstract byte[] Serialize(object payload);

        /// <summary>
        /// deserializes received byte-array to an object of TMessage
        /// </summary>
        /// <param name="messageAsBytes"></param>
        /// <returns></returns>
        protected internal abstract TMessage Deserialize(byte[] messageAsBytes);
    }
}
