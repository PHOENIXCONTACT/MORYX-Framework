// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.DriverStates;

internal class InitializingSubscriptionsState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap)
      : DriverOpcUaState(context, stateMap, StateClassification.Initializing)
{
    internal override void OnSubscriptionsInitialized()
    {
        NextState(StateRunning);
        Context.ReadDeviceSet();
    }

    internal override OpcUaNode GetNode(string identifier)
    {
        return Context.GetNode(identifier);
    }

    internal override void AddSubscription(OpcUaNode node)
    {
        Context.AddSubscriptionToSession(node);
    }

    internal override void RebrowseNodes()
    {
        Context.RemoveSubscription();
        NextState(StateBrowsingNodes);
        Context.BrowseNodes();
    }
}
