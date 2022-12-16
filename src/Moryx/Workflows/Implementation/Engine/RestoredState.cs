// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    internal class RestoredState : EngineState
    {
        public RestoredState(WorkplanEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <summary>
        /// Start the engine
        /// </summary>
        internal override void Start()
        {
            NextState(StateRunning);
            Context.ExecuteResume();
        }

        /// <summary>
        /// Return the snapshot that was used to restore the engine
        /// </summary>
        internal override WorkplanSnapshot Pause()
        {
            return Context.CurrentSnapshot;
        }

        internal override void Destroy()
        {
            ExecuteDestroy();
        }
    }
}
