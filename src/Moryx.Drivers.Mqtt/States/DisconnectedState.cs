// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.Mqtt.States;

internal class DisconnectedState : DriverMqttState
{
    public DisconnectedState(MqttDriver context, StateMap stateMap) : base(context, stateMap, StateClassification.Offline)
    {

    }

    public override void Connect()
    {
        NextState(StateConnecting);
    }

    internal override void ClientStateChanged(bool connected)
    {
        if (connected)
        {
            Context.Logger.LogWarning("Unexpected transition from Disconnected directly to connected");
            NextState(StateConnected);
        }
    }

    public override void Disconnect()
    {
        // Nothing happens
    }

}
