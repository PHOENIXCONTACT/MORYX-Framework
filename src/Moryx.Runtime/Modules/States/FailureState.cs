// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Notifications;

namespace Moryx.Runtime.Modules
{
    internal class InitializedFailureState : FailureStateBase
    {
        public InitializedFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override Task OnFailure()
        {
            return Task.CompletedTask;
        }
    }

    internal class RunningFailureState : FailureStateBase
    {
        public RunningFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override async Task OnFailure()
        {
            try
            {
                await Context.StopAsync();
            }
            catch (Exception ex)
            {
                Context.LogNotification(new ModuleNotification(Severity.Error, "Failed to stop faulty module!", ex));
            }
        }
    }

    internal abstract class FailureStateBase : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Failure;

        protected FailureStateBase(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        public override async Task OnEnterAsync(CancellationToken cancellationToken)
        {
            await OnFailure();

            try
            {
                // Regardless of the previous step we need to try destroying the container
                // This can still cause Exceptions in Dispose
                Context.Destruct();
            }
            catch (Exception ex)
            {
                Context.LogNotification(new ModuleNotification(Severity.Error, "Failed to destroy faulty container!", ex));
            }
        }

        protected abstract Task OnFailure();

        public override Task Initialize()
        {
            return NextStateAsync(StateInitializing);
        }

        public override Task Stop()
        {
            return NextStateAsync(StateStopped);
        }
    }
}
