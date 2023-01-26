// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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

        public override void Initialize()
        {
            NextState(StateInitializing);
        }

        public override void Stop()
        {
            // Stop again does not matter
        }
    }
}
