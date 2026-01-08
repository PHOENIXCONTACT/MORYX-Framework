// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.Mqtt.States;

internal class ConnectedToBrokerState : DriverMqttState
{
    public ConnectedToBrokerState(MqttDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Running)
    {

    }

    public override void Disconnect()
    {
        // Since Disconnect is an overriden member that returns void, we can only make this
        // method async void (not preferable) or use Wait here (also ugly but better).
        ExecuteDisconnect().Wait();
    }

    internal override Task SendAsync(MqttTopic topic, object message, CancellationToken cancellationToken)
    {
        return topic.OnSend(message, cancellationToken);
    }

    internal override void ConnectionToBrokerLost()
    {
        Context.Logger.Log(LogLevel.Information, "Connection to broker lost");
        NextState(StateConnecting);
        Context.ParallelOperations.ScheduleExecution(() => Context.Connect(true).Wait(), Context.ReconnectDelayMs, -1);
    }

    internal override void AddTopic(string topic)
    {
        Context.SubscribeTopicAsync(topic).Wait();
    }

    internal override void TriedConnecting(bool successful)
    {
        if (successful)
        {
            return;
        }

        base.TriedConnecting(successful);
    }
}