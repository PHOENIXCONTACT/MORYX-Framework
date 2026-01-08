// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.StateMachines;

namespace Moryx.Drivers.Mqtt.States
{
    internal abstract class DriverMqttState : SyncDriverState<MqttDriver>
    {
        protected DriverMqttState(MqttDriver context, StateMap stateMap, StateClassification classification) : base(context, stateMap, classification)
        {

        }

        protected async Task ExecuteDisconnect()
        {
            NextState(StateDisconnected);

            await Context.Disconnect();
        }

        internal virtual Task SendAsync(MqttTopic topic, object message, CancellationToken cancellationToken)
        {
            throw new InvalidOperationException($"Current state {GetType().Name} can net send message of type {message.GetType().Name}");
        }

        internal virtual void TriedConnecting(bool successful)
        {
            InvalidState();
        }

        internal virtual void ConnectionToBrokerLost()
        {
            //do nothing
        }

        internal virtual void AddTopic(string mqttTopic)
        {
            //do nothing
        }

        [StateDefinition(typeof(DisconnectedState), IsInitial = true)]
        protected const int StateDisconnected = 10;

        [StateDefinition(typeof(ConnectingToBrokerState))]
        protected const int StateConnecting = 20;

        [StateDefinition(typeof(ConnectedToBrokerState))]
        protected const int StateConnected = 30;
    }
}

