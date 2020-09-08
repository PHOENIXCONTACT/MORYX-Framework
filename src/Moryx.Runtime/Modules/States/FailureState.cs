// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using Moryx.Notifications;

namespace Moryx.Runtime.Modules
{
    internal class InitializedFailureState : FailureStateBase
    {
        public InitializedFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override void OnFailure()
        {
        }
    }

    internal class RunningFailureState : FailureStateBase
    {
        public RunningFailureState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        protected override void OnFailure()
        {
            try
            {
                Context.Stop();
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

        public override void OnEnter()
        {
            OnFailure();

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

        protected abstract void OnFailure();

        public override void Initialize()
        {
            NextState(StateInitializing);
        }

        public override void Stop()
        {
            NextState(StateStopped);
        }
    }
}
