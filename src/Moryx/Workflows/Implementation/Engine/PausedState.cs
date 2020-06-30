// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Workflows
{
    internal class PausedState : EngineState
    {
        public PausedState(WorkflowEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <summary>
        /// Pause exeuction on the engine
        /// </summary>
        internal override WorkflowSnapshot Pause()
        {
            return Context.CurrentSnapshot;
        }

        internal override void Restore(WorkflowSnapshot snapshot)
        {
            if(snapshot != Context.CurrentSnapshot)
                throw new InvalidOperationException("Can not restore paused engine with different snapshot. Create a new instance instead!");
        }

        internal override void Start()
        {
            NextState(StateRunning);
            Context.ExecuteResume();
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}
