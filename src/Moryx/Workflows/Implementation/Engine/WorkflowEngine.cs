// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moryx.StateMachines;

namespace Moryx.Workflows
{
    internal class WorkflowEngine : IMonitoredEngine, IStateContext
    {
        public WorkflowEngine()
        {
            StateMachine.Initialize(this).With<EngineState>();
        }

        internal EngineState State { get; private set; }

        void IStateContext.SetState(IState state)
        {
            State = (EngineState) state;
        }

        ///
        public IWorkflow ExecutedWorkflow { get; private set; }

        ///
        public IWorkplanContext Context { get; set; }

        /// <summary>
        /// Reference to the snapshot, if there is one
        /// </summary>
        internal WorkflowSnapshot CurrentSnapshot { get; set; }

        ///
        public void Initialize(IWorkflow workflow)
        {
            State.Initialize(workflow);
        }

        internal void ExecuteInitialize(IWorkflow workflow)
        {
            ExecutedWorkflow = workflow;
            // Register to events of observable transitions
            foreach (var transition in ExecutedWorkflow.Transitions)
            {
                transition.Initialize();
                if (transition is IObservableTransition)
                    ((IObservableTransition)transition).Triggered += OnTransitionTriggered;
            }
            // Register to events of exit places
            foreach (var place in ExecutedWorkflow.Places)
            {
                place.TokenAdded += OnPlaceReached;
            }
        }

        private void OnPlaceReached(object sender, IToken token)
        {
            var place = (IPlace) sender;
            // Check if the engine completed its execution
            if (place.Classification.HasFlag(NodeClassification.Exit)
              && (place.Classification == NodeClassification.Failed || token is MainToken))
            {
                State.Completed();
                Completed(this, place);
            }
            else if(place.Classification == NodeClassification.Intermediate)
            {
                PlaceReached?.Invoke(this, place);
            }
        }

        private void OnTransitionTriggered(object sender, EventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            TransitionTriggered(this, (IObservableTransition)sender);
        }

        ///
        public void Start()
        {
            State.Start();
        }

        internal void ExecuteStart()
        {
            foreach (var startPlace in ExecutedWorkflow.StartPlaces())
            {
                startPlace.Add(new MainToken());
            }
        }

        internal void ExecuteResume()
        {
            // Resume all holders that carry a token
            foreach (var holder in GetAllHolders().Where(IsRelevantHolder).ToArray())
            {
                holder.Resume();
            }
        }

        #region Pause and restore

        ///
        public WorkflowSnapshot Pause()
        {
            return State.Pause();
        }

        internal void ExecutePause()
        {
            // Create snapshot with workplan name
            var snapShot = new WorkflowSnapshot { WorkplanName = ExecutedWorkflow.Workplan.Name };

            // Pause all places or transitions with tokens
            foreach (var holder in GetAllHolders().Where(IsRelevantHolder))
            {
                holder.Pause();
            }

            // Await all transitions to finish up currently executed code
            while (ExecutedWorkflow.Transitions.Any(transition => transition.Executing))
            {
                // This will force a context switch to give CPU time to the transitions
                Thread.Sleep(1);
            }

            // Extract snapshot from holders
            snapShot.Holders = (from holder in GetAllHolders()
                                let tokens = holder.Tokens.ToArray()
                                where tokens.Length > 0
                                select new HolderSnapshot
                                {
                                    HolderId = holder.Id,
                                    Tokens = tokens,
                                    HolderState = holder.InternalState
                                }).ToArray();

            CurrentSnapshot = snapShot;
        }

        /// <summary>
        /// Indicate that this place is relevant for Pause and Resume operations
        /// </summary>
        private static bool IsRelevantHolder(ITokenHolder holder)
        {
            return holder is IPlace || holder.Tokens.Any();
        }

        /// <summary>
        /// Restore the workflow state from a snapshot
        /// </summary>
        /// <param name="snapshot"></param>
        public void Restore(WorkflowSnapshot snapshot)
        {
            State.Restore(snapshot);
        }

        internal void ExecuteRestore()
        {
            var allHolder = GetAllHolders();
            foreach (var holder in CurrentSnapshot.Holders)
            {
                var match = allHolder.First(h => h.Id == holder.HolderId);
                match.Tokens = holder.Tokens;
                match.InternalState = holder.HolderState;
            }
        }

        private ITokenHolder[] GetAllHolders()
        {
            IEnumerable<ITokenHolder> holderPlaces = ExecutedWorkflow.Places;
            IEnumerable<ITokenHolder> holderTransitions = ExecutedWorkflow.Transitions;
            return holderPlaces.Union(holderTransitions).ToArray();
        }

        #endregion

        ///
        public event EventHandler<IPlace> PlaceReached;

        ///
        public event EventHandler<ITransition> TransitionTriggered;

        ///
        public event EventHandler<IPlace> Completed;

        ///
        public void Dispose()
        {
            State.Destroy();
        }

        internal void ExecuteDispose()
        {
            // Unregister from each observable transitions
            foreach (var transition in ExecutedWorkflow.Transitions.OfType<IObservableTransition>())
            {
                transition.Triggered -= OnTransitionTriggered;
            }
            // Unregister from events of exit places
            foreach (var place in ExecutedWorkflow.Places)
            {
                place.TokenAdded -= OnPlaceReached;
            }
            ExecutedWorkflow = null;
        }
    }
}
