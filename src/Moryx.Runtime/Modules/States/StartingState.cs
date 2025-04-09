// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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

        public override void OnEnter()
        {
            try
            {
                Context.Start();
                NextState(StateRunning);
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
            // We are already starting
        }

        public override void Stop()
        {
            // Nothing to do here
        }
    }
}
