// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.Drivers.OpcUa.Nodes;

namespace Moryx.Drivers.OpcUa.States;

internal class RunningState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Running)
{
    public override async Task OnEnterAsync(CancellationToken cancellationToken)
    {
        Context.PublishRunningState();
        await base.OnEnterAsync(cancellationToken);
    }

    internal override Task<OpcUaNode> GetNode(string identifier)
    {
        return Context.GetNodeAsync(identifier);
    }

    internal override Task<DataValueResult> ReadValueAsync(string identifier, CancellationToken cancellationToken)
    {
        return Context.OnReadValueOfNode(identifier, cancellationToken);
    }

    internal override async Task AddSubscription(string nodeId)
    {
        var node = await GetNode(nodeId);
        await Context.AddSubscriptionToSession(node);
    }

    internal override Task WriteNodeAsync(OpcUaNode node, object payload, CancellationToken cancellationToken)
    {
        return Context.OnWriteNode(node, payload, cancellationToken);
    }
}
