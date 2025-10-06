// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.Mqtt.DriverStates
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
                Context.ParallelOperations.ScheduleExecution(async () => await Context.Connect(false), Context.ReconnectDelayMs, -1);
        }

        public override void Disconnect()
        {
            // Since Disconnect is an overriden member that returns void, we can only make this
            // method async void (not preferable) or use Wait here (also ugly but better).
            ExecuteDisconnect().Wait();
        }
    }
}
