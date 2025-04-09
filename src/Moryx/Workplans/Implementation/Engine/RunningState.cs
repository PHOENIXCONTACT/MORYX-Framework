// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    internal class RunningState : EngineState
    {
        public RunningState(WorkplanEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        internal override WorkplanSnapshot Pause()
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
