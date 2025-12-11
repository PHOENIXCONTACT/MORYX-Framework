// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class StartingState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Starting;

        public StartingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        public override async Task OnEnterAsync()
        {
            try
            {
                await Context.StartAsync();
                await NextStateAsync(StateRunning);
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
            // We are already starting
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }
    }
}
