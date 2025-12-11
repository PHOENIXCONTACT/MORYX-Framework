// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class StoppedState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Stopped;

        public StoppedState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
        {
        }

        public override Task Initialize()
        {
            return NextStateAsync(StateInitializing);
        }

        public override Task Stop()
        {
            // Stop again does not matter
            return Task.CompletedTask;
        }
    }
}
