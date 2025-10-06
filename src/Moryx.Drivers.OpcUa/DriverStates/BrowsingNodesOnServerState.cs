// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.DriverStates;

internal class BrowsingNodesOnServerState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Initializing)
{
    internal override void RebrowseNodes()
    {
        // do nothing, since nodes are already browsed
    }

    internal override void OnBrowsingNodesCompleted()
    {
        NextState(StateInitializingSubscriptions);
        Context.SubscribeSavedNodes();
    }
}