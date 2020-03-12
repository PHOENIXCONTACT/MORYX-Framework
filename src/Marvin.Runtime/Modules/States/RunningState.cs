// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Marvin.Runtime.Modules
{
    internal class RunningState : ServerModuleStateBase
    {
        public RunningState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap, ServerModuleState.Running)
        {
        }

        public override void OnEnter()
        {
            try
            {
                Context.Started();
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                NextState(StateRunningFailure);
            }
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // Already started
        }

        public override void Stop()
        {
            NextState(StateRunningStopping);
        }

        public override void ValidateHealthState()
        {
            // Health state should be okay!
        }
    }
}
