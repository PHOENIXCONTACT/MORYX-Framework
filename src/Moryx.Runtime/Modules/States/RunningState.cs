// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class RunningState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Running;

        public RunningState(IServerModuleStateContext context, StateMap stateMap) : base(context, stateMap)
        {
        }

        public override async Task OnEnterAsync()
        {
            try
            {
                Context.Started();
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                await NextStateAsync(StateRunningFailure);
            }
        }

        public override Task Initialize()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Start()
        {
            // Already started
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            return NextStateAsync(StateRunningStopping);
        }

        public override void ValidateHealthState()
        {
            // Health state should be okay!
        }
    }
}
