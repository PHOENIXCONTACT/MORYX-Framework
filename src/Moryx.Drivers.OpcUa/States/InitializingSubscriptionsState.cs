// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.States;

internal class InitializingSubscriptionsState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap)
      : DriverOpcUaState(context, stateMap, StateClassification.Initializing)
{
    internal override Task OnSubscriptionsInitializedAsync()
    {
        NextState(StateRunning);
        Context.ReadDeviceSet();
        return Task.CompletedTask;
    }

    internal override OpcUaNode GetNode(string identifier)
    {
        return Context.GetNodeAsync(identifier).GetAwaiter().GetResult();
    }

    internal override void AddSubscription(OpcUaNode node)
    {
        Context.AddSubscriptionToSession(node);
    }

    internal override async Task RebrowseNodesAsync()
    {
        Context.RemoveSubscription();
        NextState(StateBrowsingNodes);
        await Context.BrowseNodesAsync();
    }
}
