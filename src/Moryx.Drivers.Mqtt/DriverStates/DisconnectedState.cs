// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.Mqtt.DriverStates
{
    internal class DisconnectedState: DriverMqttState
    {
        public DisconnectedState(MqttDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Offline)
        {

        }

        public override void Connect()
        {
            NextState(StateConnecting);

            Context.Connect().Wait();
            
        }
        public override void Disconnect()
        {
            // Nothing happens
        }

        internal override void TriedConnecting(bool successful)
        {
            // Nothing happens
        }
    }
}
