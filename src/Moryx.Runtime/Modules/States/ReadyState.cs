// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Runtime.Modules
{
    internal class ReadyState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Ready;

        public ReadyState(IServerModuleStateContext context, StateMap stateMap)
            : base(context, stateMap)
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
