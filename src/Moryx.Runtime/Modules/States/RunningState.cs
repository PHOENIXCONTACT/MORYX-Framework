// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class RunningState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Running;

        public RunningState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
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
