// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Runtime.Modules
{
    internal class InitializingState : ServerModuleStateBase
    {
        public override ServerModuleState Classification => ServerModuleState.Initializing;

        public InitializingState(IServerModuleStateContext context, StateMap stateMap) 
            : base(context, stateMap)
        {
        }

        public override void OnEnter()
        {
            try
            {
                Context.Initialize();
                NextState(StateReady);
            }
            catch (Exception ex)
            {
                Context.ReportError(ex);
                NextState(StateInitializedFailure);
            }
        }

        public override void Initialize()
        {
            // Nothing to do here
        }

        public override void Start()
        {
            // Nothing to do here
        }

        public override void Stop()
        {
            // Nothing to do here
        }
    }
}
