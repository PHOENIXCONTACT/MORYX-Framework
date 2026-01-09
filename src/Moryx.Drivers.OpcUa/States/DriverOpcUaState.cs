// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.StateMachines;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.States;

internal abstract class DriverOpcUaState(OpcUaDriver context, StateBase.StateMap stateMap, StateClassification classification) : SyncDriverState<OpcUaDriver>(context, stateMap, classification)
{
    public override void Connect()
    {
        InvalidState();
    }

    internal virtual Task OnConnectingCompletedAsync(bool successfull)
    {
        return InvalidStateAsync();
    }

    internal virtual Task RebrowseNodesAsync()
    {
        return InvalidStateAsync();
    }

    internal virtual Task OnBrowsingNodesCompletedAsync()
    {
        return InvalidStateAsync();
    }

    internal virtual Task OnSubscriptionsInitializedAsync()
    {
        return InvalidStateAsync();
    }

    public override void Disconnect()
    {
        Context.Disconnect();
        NextState(StateDisconnected);
    }

    internal virtual async Task OnConnectionLostAsync(KeepAliveEventArgs e)
    {
        Context.RemoveSubscription();
        NextState(StateReconnecting);
        await Context.Reconnect(e);
    }

    internal virtual OpcUaNode GetNode(string identifier)
    {
        return Context.GetNotInitializedNode(identifier);
    }

    internal virtual void AddSubscription(OpcUaNode node)
    {
        Context.SaveSubscriptionToBeAdded(node);
    }

    internal virtual Task<DataValueResult> ReadValueAsync(string identifier, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException();
    }

    internal virtual Task WriteNodeAsync(OpcUaNode node, object payload, CancellationToken cancellationToken)
    {
        return InvalidStateAsync();
    }

    internal virtual void Send()
    {
        InvalidState();
    }

    [StateDefinition(typeof(DisconnectedState), IsInitial = true)]
    protected const int StateDisconnected = 10;

    [StateDefinition(typeof(ConnectingToServerState))]
    protected const int StateConnecting = 20;

    [StateDefinition(typeof(BrowsingNodesOnServerState))]
    protected const int StateBrowsingNodes = 30;

    [StateDefinition(typeof(InitializingSubscriptionsState))]
    protected const int StateInitializingSubscriptions = 40;

    [StateDefinition(typeof(RunningState))]
    protected const int StateRunning = 50;

    [StateDefinition(typeof(ReconnectingState))]
    protected const int StateReconnecting = 60;
}
