// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class RunningStoppingState : StoppingStateBase
    {
        public RunningStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override async Task OnStopping(CancellationToken cancellationToken)
        {
            try
            {
                await Context.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                await NextStateAsync(StateRunningFailure, cancellationToken);
            }
        }
    }

    internal class ReadyStoppingState : StoppingStateBase
    {
        public ReadyStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override Task OnStopping(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal abstract class StoppingStateBase : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Stopping;

        protected StoppingStateBase(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        public override async Task OnEnterAsync(CancellationToken cancellationToken)
        {
            await OnStopping(cancellationToken);

            try
            {
                // Regardless of the previous state we need to destruct the container
                Context.Destruct();
                await NextStateAsync(StateStopped, cancellationToken);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                await NextStateAsync(StateInitializedFailure, cancellationToken);
            }
        }

        protected abstract Task OnStopping(CancellationToken cancellationToken);

        public override Task Initialize(CancellationToken cancellationToken)
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Start(CancellationToken cancellationToken)
        {
            // Not possible here
            return Task.CompletedTask;
        }

        public override Task Stop(CancellationToken cancellationToken)
        {
            // We are already stopping
            return Task.CompletedTask;
        }
    }
}
