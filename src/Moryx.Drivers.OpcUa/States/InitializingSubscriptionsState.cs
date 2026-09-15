// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.Drivers.OpcUa.Nodes;

namespace Moryx.Drivers.OpcUa.States;

internal class InitializingSubscriptionsState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap)
      : DriverOpcUaState(context, stateMap, StateClassification.Initializing)
{
    public override async Task OnEnterAsync(CancellationToken cancellationToken)
    {
        await base.OnEnterAsync(cancellationToken);
        await Context.SubscribeSavedNodesAsync(cancellationToken);
    }

    internal override async Task OnSubscriptionsInitializedAsync(CancellationToken cancellationToken)
    {
        await NextStateAsync(StateRunning, cancellationToken);
        await Context.ReadDeviceSetAsync(cancellationToken);
    }

    internal override Task<OpcUaNode> GetNode(string identifier)
    {
        return Context.GetNodeAsync(identifier);
    }

    internal override async Task AddSubscription(string nodeId)
    {
        var node = await GetNode(nodeId);
        await Context.AddSubscriptionToSession(node);
    }
}
