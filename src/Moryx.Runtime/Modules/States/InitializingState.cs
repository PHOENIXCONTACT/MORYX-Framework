// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class InitializingState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Initializing;

        public InitializingState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }
        public override async Task OnEnterAsync()
        {
            try
            {
                await Context.InitializeAsync();
                await NextStateAsync(StateReady);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                await NextStateAsync(StateInitializedFailure);
            }
        }
        public override Task Initialize()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Start()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            // Nothing to do here
            return Task.CompletedTask;
        }
    }
}
