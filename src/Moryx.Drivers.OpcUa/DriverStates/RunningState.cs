// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.DriverStates;

internal class RunningState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Running)
{
    internal override void RebrowseNodes()
    {
        Context.RemoveSubscription();
        NextState(StateBrowsingNodes);
        Context.BrowseNodes();
    }

    internal override OpcUaNode GetNode(string identifier)
    {
        return Context.GetNode(identifier);
    }

    internal override DataValue ReadValue(string identifier)
    {
        return Context.OnReadValueOfNode(identifier);
    }

    internal override void AddSubscription(OpcUaNode node)
    {
        Context.AddSubscriptionToSession(node);
    }

    internal override void WriteNode(OpcUaNode node, object payload)
    {
        Context.OnWriteNode(node, payload);
    }
}
