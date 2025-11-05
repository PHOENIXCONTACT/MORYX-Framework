// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.Mqtt.States
{
    internal class ConnectingToBrokerState : DriverMqttState
    {
        public ConnectingToBrokerState(MqttDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Initializing)
        {

        }

        internal override void TriedConnecting(bool successful)
        {
            if (successful)
                NextState(StateConnected);
            else
                Context.ParallelOperations.ScheduleExecution(() => Context.Connect(false).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        Context.Logger?.LogWarning(t.Exception, "Failed to connect to broker");
                    }
                }, scheduler: TaskScheduler.Current), Context.ReconnectDelayMs, -1);
        }

        public override void Disconnect()
        {
            // Since Disconnect is an overriden member that returns void, we can only make this
            // method async void (not preferable) or use Wait here (also ugly but better).
            ExecuteDisconnect().Wait();
        }
    }
}
