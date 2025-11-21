// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.States;

internal class ReconnectingState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Offline)
{
    internal override void OnConnectionLost(KeepAliveEventArgs e)
    {
        //do nothing
    }

    internal override void OnConnectingCompleted(bool successfull)
    {
        if (successfull)
        {
            NextState(StateBrowsingNodes);
            Context.BrowseNodes();
        }

    }
}
