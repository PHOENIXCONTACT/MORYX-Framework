// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Drivers;

namespace Moryx.Drivers.OpcUa.States;

internal class ConnectingToServerState(OpcUaDriver context, StateMachines.StateBase.StateMap stateMap) : DriverOpcUaState(context, stateMap, StateClassification.Offline)
{
    public override async Task OnEnterAsync(CancellationToken cancellationToken)
    {
        await base.OnEnterAsync(cancellationToken);
        await Context.TryConnect(true, cancellationToken);
    }

    internal override async Task OnConnectingCompletedAsync(bool successfull, CancellationToken cancellationToken)
    {
        if (successfull)
        {
            await NextStateAsync(StateInitializingSubscriptions, cancellationToken);
        }
        else
        {
            await Context.TryConnect(false, cancellationToken);
        }

    }
}
