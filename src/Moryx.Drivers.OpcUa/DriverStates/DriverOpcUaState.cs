// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moryx.AbstractionLayer.Drivers;
using Moryx.StateMachines;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.DriverStates;

internal abstract class DriverOpcUaState(OpcUaDriver context, StateBase.StateMap stateMap, StateClassification classification) : DriverState<OpcUaDriver>(context, stateMap, classification)
{
    public override void Connect()
    {
        InvalidState();
    }

    internal virtual void OnConnectingCompleted(bool successfull)
    {
        InvalidState();
    }

    internal virtual void RebrowseNodes()
    {
        InvalidState();
    }

    internal virtual void OnBrowsingNodesCompleted()
    {
        InvalidState();
    }

    internal virtual void OnSubscriptionsInitialized()
    {
        InvalidState();
    }

    public override void Disconnect()
    {
        Context.Disconnect();
        NextState(StateDisconnected);
    }

    internal virtual void OnConnectionLost(KeepAliveEventArgs e)
    {
        Context.RemoveSubscription();
        NextState(StateReconnecting);
        Context.Reconnect(e);
    }

    internal virtual OpcUaNode GetNode(string identifier)
    {
        return Context.GetNotInitializedNode(identifier);
    }

    internal virtual void AddSubscription(OpcUaNode node)
    {
        Context.SaveSubscriptionToBeAdded(node);
    }

    internal virtual DataValue ReadValue(string identifier)
    {
        throw new InvalidOperationException();
    }

    internal virtual void WriteNode(OpcUaNode node, object payload)
    {
        InvalidState();
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
