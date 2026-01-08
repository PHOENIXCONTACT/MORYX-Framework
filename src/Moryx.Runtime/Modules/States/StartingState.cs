// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules;

internal class StartingState : ServerModuleStateBase
{
    public override ServerModuleState Classification => ServerModuleState.Starting;

    public StartingState(IServerModuleStateContext context, StateMap stateMap)
        : base(context, stateMap)
    {
    }

    public override async Task OnEnterAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.StartAsync(cancellationToken);
            await NextStateAsync(StateRunning, cancellationToken);
        }
        catch (Exception ex)
        {
            Context.ReportError(ex);
            await NextStateAsync(StateRunningFailure, cancellationToken);
        }
    }

    public override Task Initialize(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }

    public override Task Start(CancellationToken cancellationToken)
    {
        // We are already starting
        return Task.CompletedTask;
    }

    public override Task Stop(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }
}