// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.States;

internal class ReconnectingState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Offline)
{
    internal override Task OnConnectionLostAsync(KeepAliveEventArgs e)
    {
        //do nothing
        return Task.CompletedTask;
    }

    internal override async Task OnConnectingCompletedAsync(bool successfull)
    {
        if (successfull)
        {
            NextState(StateBrowsingNodes);
            await Context.BrowseNodesAsync();
        }

    }
}
