// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.StateMachines;

namespace Moryx.Drivers.Mqtt.States;

internal abstract class DriverMqttState : SyncDriverState<MqttDriver>
{
    protected DriverMqttState(MqttDriver context, StateMap stateMap, StateClassification classification) : base(context, stateMap, classification)
    {

    }

    protected async Task ExecuteDisconnect()
    {
        await Context.Disconnect();
    }

    internal virtual Task SendAsync(MqttTopic topic, object message, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException($"Current state {GetType().Name} can net send message of type {message.GetType().Name}.");
    }

    internal virtual void ClientStateChanged(bool connected)
    {
        
    }

    internal virtual void AddTopic(string mqttTopic)
    {
        //do nothing
    }

    internal virtual void ReconnectTimerElapsed()
    {
        // do nothing
    }

    [StateDefinition(typeof(DisconnectedState), IsInitial = true)]
    protected const int StateDisconnected = 10;

    
    [StateDefinition(typeof(ReconnectBackoffState))]
    protected const int ReconnectBackoffState = 15;

    [StateDefinition(typeof(ConnectingToBrokerState))]
    protected const int StateConnecting = 20;

    [StateDefinition(typeof(ConnectedToBrokerState))]
    protected const int StateConnected = 30;
}
