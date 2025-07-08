﻿// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.DriverStates;

internal class ConnectingToServerState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Offline)
{
    internal override void OnConnectingCompleted(bool successfull)
    {
        if (successfull)
        {
            NextState(StateBrowsingNodes);
            Context.BrowseNodes();
        }
        else
        {
            Context.TryConnect(false);
        }

    }
}
