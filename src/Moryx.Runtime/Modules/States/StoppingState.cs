// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Modules
{
    internal class RunningStoppingState : StoppingStateBase
    {
        public RunningStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override void OnStopping()
        {
            try
            {
                Context.Stop();
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                NextState(StateRunningFailure);
            }
        }
    }

    internal class ReadyStoppingState : StoppingStateBase
    {
        public ReadyStoppingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        protected override void OnStopping()
        {
        }
    }

    internal abstract class StoppingStateBase : ServerModuleStateBase
    {
        protected StoppingStateBase(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Stopping)
        {
        }

        public override void OnEnter()
        {
            OnStopping();

            try
            {
                // Regardless of the previous state we need to destruct the container
                Context.Destruct();
                NextState(StateStopped);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                NextState(StateInitializedFailure);
            }
        }

        protected abstract void OnStopping();

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // Not possible here
        }

        public override void Stop()
        {
            // We are already stopping
        }
    }
}
