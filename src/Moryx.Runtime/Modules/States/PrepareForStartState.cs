// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules;

internal class PrepareForStartState : ServerModuleStateBase
{
    public override ServerModuleState Classification => ServerModuleState.Initializing;

    public PrepareForStartState(IServerModuleStateContext context, StateMap stateMap)
        : base(context, stateMap)
    {
    }
    public override async Task OnEnterAsync(CancellationToken cancellationToken)
    {
        try
        {
            await Context.InitializeAsync(cancellationToken);
            //await Context.StartAsync(cancellationToken);
            await NextStateAsync(StateStarting, cancellationToken);
        }
        catch (Exception ex)
        {
            Context.ReportError(ex);
            await NextStateAsync(StateInitializedFailure, cancellationToken);
        }
    }
    public override Task Initialize(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }

    public override Task Start(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }

    public override Task Stop(CancellationToken cancellationToken)
    {
        // Nothing to do here
        return Task.CompletedTask;
    }
}
