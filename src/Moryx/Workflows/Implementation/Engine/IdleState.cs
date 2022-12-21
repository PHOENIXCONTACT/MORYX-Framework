// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans
{
    internal class IdleState : EngineState
    {
        public IdleState(WorkplanEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <summary>
        /// Initialize the engine
        /// </summary>
        internal override void Initialize(IWorkplanInstance workplanInstance)
        {
            NextState(StateReady);
            Context.ExecuteInitialize(workplanInstance);
        }

        internal override void Destroy()
        {
            Context.ExecuteDispose();
        }
    }
}
