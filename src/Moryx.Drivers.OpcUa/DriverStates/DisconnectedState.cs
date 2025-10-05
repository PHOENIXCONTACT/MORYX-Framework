// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer.Drivers;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.DriverStates;

internal class DisconnectedState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Offline)
{
    public override void Connect()
    {
        NextState(StateConnecting);
        Context.TryConnect(true);
    }

    internal override void OnConnectionLost(KeepAliveEventArgs e)
    {
        InvalidState();
    }
}
