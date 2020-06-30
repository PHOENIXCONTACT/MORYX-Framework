// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Workflows
{
    internal class RunningState : EngineState
    {
        public RunningState(WorkflowEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        internal override WorkflowSnapshot Pause()
        {
            NextState(StatePaused);
            Context.ExecutePause();
            return Context.CurrentSnapshot;
        }

        internal override void Completed()
        {
            NextState(StateIdle);
        }

        internal override void Destroy()
        {
            Context.ExecutePause();
            ExecuteDestroy();
        }
    }
}
