// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class ReadyState : ServerModuleStateBase
    {
        public ReadyState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap, ServerModuleState.Ready)
        {
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            NextState(StateStarting);
        }

        public override void Stop()
        {
            NextState(StateReadyStopping);
        }
    }
}
