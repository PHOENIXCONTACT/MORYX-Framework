// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workflows
{
    internal class IdleState : EngineState
    {
        public IdleState(WorkflowEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <summary>
        /// Initialize the engine
        /// </summary>
        internal override void Initialize(IWorkflow workflow)
        {
            NextState(StateReady);
            Context.ExecuteInitialize(workflow);
        }

        internal override void Destroy()
        {
            Context.ExecuteDispose();
        }
    }
}
