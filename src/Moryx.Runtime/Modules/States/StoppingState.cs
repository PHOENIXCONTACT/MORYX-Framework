// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class RunningStoppingState : StoppingStateBase
    {
        public RunningStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override async Task OnStopping()
        {
            try
            {
                await Context.Stop();
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                await NextStateAsync(StateRunningFailure);
            }
        }
    }

    internal class ReadyStoppingState : StoppingStateBase
    {
        public ReadyStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override Task OnStopping()
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

        public override async Task OnEnterAsync()
        {
            await OnStopping();

            try
            {
                // Regardless of the previous state we need to destruct the container
                Context.Destruct();
                await NextStateAsync(StateStopped);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                await NextStateAsync(StateInitializedFailure);
            }
        }

        protected abstract Task OnStopping();

        public override Task Initialize()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Start()
        {
            // Not possible here
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            // We are already stopping
            return Task.CompletedTask;
        }
    }
}
