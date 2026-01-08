// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Notifications;

namespace Moryx.Runtime.Modules
{
    internal class InitializedFailureState : FailureStateBase
    {
        public InitializedFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override Task OnFailure(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    internal class RunningFailureState : FailureStateBase
    {
        public RunningFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override async Task OnFailure(CancellationToken cancellationToken)
        {
            try
            {
                await Context.StopAsync(cancellationToken);
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
            await OnFailure(cancellationToken);

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

        protected abstract Task OnFailure(CancellationToken cancellationToken);

        public override Task Initialize(CancellationToken cancellationToken)
        {
            return NextStateAsync(StateInitializing, cancellationToken);
        }

        public override Task Stop(CancellationToken cancellationToken)
        {
            return NextStateAsync(StateStopped, cancellationToken);
        }
    }
}
