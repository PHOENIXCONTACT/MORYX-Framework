// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.StateMachines;

namespace Moryx.Workplans
{
    /// <summary>
    /// State pattern for the engine
    /// </summary>
    internal abstract class EngineState : StateBase<WorkplanEngine>
    {
        protected EngineState(WorkplanEngine context, StateMap stateMap) : base(context, stateMap)
        {
        }

        /// <summary>
        /// Initialize the engine
        /// </summary>
        internal virtual void Initialize(IWorkplanInstance workplanInstance)
        {
            InvalidState();
        }

        /// <summary>
        /// Start the engine
        /// </summary>
        internal virtual void Start()
        {
            InvalidState();
        }

        /// <summary>
        /// Transition called when engine was completed
        /// </summary>
        internal virtual void Completed()
        {
            InvalidState();
        }

        /// <summary>
        /// Pause exeuction on the engine
        /// </summary>
        internal virtual WorkplanSnapshot Pause()
        {
            InvalidState();
            return null;
        }

        /// <summary>
        /// Restore the workplan instance from a snapshot
        /// </summary>
        internal virtual void Restore(WorkplanSnapshot snapshot)
        {
            InvalidState();
        }

        /// <summary>
        /// Destroy the engine instance
        /// </summary>
        internal virtual void Destroy()
        {
            InvalidState();
        }

        protected void ExecuteDestroy()
        {
            NextState(StateIdle);
            Context.ExecuteDispose();
        }

        [StateDefinition(typeof(IdleState), IsInitial = true)]
        protected const int StateIdle = 0;

        [StateDefinition(typeof(ReadyState))]
        protected const int StateReady = 10;

        [StateDefinition(typeof(RestoredState))]
        protected const int StateRestored = 15;

        [StateDefinition(typeof(RunningState))]
        protected const int StateRunning = 20;

        [StateDefinition(typeof(PausedState))]
        protected const int StatePaused = 30;
    }
}
