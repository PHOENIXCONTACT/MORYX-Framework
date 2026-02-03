// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;
using Moryx.Drivers.OpcUa.Nodes;
using Moryx.StateMachines;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.States;

internal abstract class DriverOpcUaState(OpcUaDriver context, StateBase.StateMap stateMap, StateClassification classification) : AsyncDriverState<OpcUaDriver>(context, stateMap, classification)
{
    public readonly SemaphoreSlim Semaphore = new(1, 1);

    public override Task ConnectAsync(CancellationToken cancellationToken)
    {
        return InvalidStateAsync();
    }

    internal virtual Task OnConnectingCompletedAsync(bool successfull, CancellationToken cancellationToken)
    {
        return InvalidStateAsync();
    }

    internal virtual Task OnSubscriptionsInitializedAsync(CancellationToken cancellationToken)
    {
        return InvalidStateAsync();
    }

    public override Task DisconnectAsync(CancellationToken cancellationToken)
    {
        Context.Disconnect();
        return NextStateAsync(StateDisconnected, cancellationToken);
    }

    internal virtual async Task OnConnectionLostAsync(KeepAliveEventArgs e, CancellationToken cancellationToken)
    {
        Context.RemoveSubscription();
        await NextStateAsync(StateReconnecting, cancellationToken);
        Context.Reconnect(e);
    }

    internal virtual OpcUaNode GetNode(string identifier)
    {
        return null;
    }

    internal virtual void AddSubscription(string nodeId)
    {
        Context.SaveSubscriptionToBeAdded(nodeId);
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

    [StateDefinition(typeof(InitializingSubscriptionsState))]
    protected const int StateInitializingSubscriptions = 40;

    [StateDefinition(typeof(RunningState))]
    protected const int StateRunning = 50;

    [StateDefinition(typeof(ReconnectingState))]
    protected const int StateReconnecting = 60;
}
