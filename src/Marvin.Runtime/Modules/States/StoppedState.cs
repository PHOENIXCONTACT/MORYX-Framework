// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Runtime.Modules
{
    internal class StoppedState : ServerModuleStateBase
    {
        public StoppedState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Stopped)
        {
        }

        public override void Initialize()
        {
            NextState(StateInitializing);
        }

        public override void Stop()
        {
            // Stop again doent matter
        }
    }
}
