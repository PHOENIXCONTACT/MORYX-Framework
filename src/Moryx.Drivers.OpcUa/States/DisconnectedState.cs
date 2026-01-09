// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.States;

internal class DisconnectedState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Offline)
{
    public override void Connect()
    {
        NextState(StateConnecting);
        Context.TryConnect(true).GetAwaiter().GetResult();
    }

    internal override Task OnConnectionLostAsync(KeepAliveEventArgs e)
    {
        return InvalidStateAsync();
    }
}
