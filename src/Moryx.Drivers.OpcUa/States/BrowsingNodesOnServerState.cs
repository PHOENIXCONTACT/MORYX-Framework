// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.States;

internal class BrowsingNodesOnServerState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Initializing)
{
    internal override Task RebrowseNodesAsync()
    {
        // do nothing, since nodes are already browsed
        return Task.CompletedTask;
    }

    internal override async Task OnBrowsingNodesCompletedAsync()
    {
        NextState(StateInitializingSubscriptions);
        await Context.SubscribeSavedNodesAsync();
    }
}
