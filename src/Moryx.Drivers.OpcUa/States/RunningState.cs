// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.States;

internal class RunningState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Running)
{
    internal override async Task RebrowseNodesAsync()
    {
        Context.RemoveSubscription();
        NextState(StateBrowsingNodes);
        await Context.BrowseNodesAsync();
    }

    internal override OpcUaNode GetNode(string identifier)
    {
        return Context.GetNodeAsync(identifier).GetAwaiter().GetResult();
    }

    internal override Task<DataValueResult> ReadValueAsync(string identifier, CancellationToken cancellationToken)
    {
        return Context.OnReadValueOfNode(identifier, cancellationToken);
    }

    internal override void AddSubscription(OpcUaNode node)
    {
        Context.AddSubscriptionToSession(node);
    }

    internal override Task WriteNodeAsync(OpcUaNode node, object payload, CancellationToken cancellationToken)
    {
        return Context.OnWriteNode(node, payload, cancellationToken);
    }
}
