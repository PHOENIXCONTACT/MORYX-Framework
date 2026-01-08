// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules;

internal class ReadyState : ServerModuleStateBase
{
    public override ServerModuleState Classification => ServerModuleState.Ready;

    public ReadyState(IServerModuleStateContext context, StateMap stateMap)
        : base(context, stateMap)
    {
    }

    public override Task Initialize(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }

    public override Task Start(CancellationToken cancellationToken)
    {
        return NextStateAsync(StateStarting, cancellationToken);
    }

    public override Task Stop(CancellationToken cancellationToken)
    {
        return NextStateAsync(StateReadyStopping, cancellationToken);
    }
}