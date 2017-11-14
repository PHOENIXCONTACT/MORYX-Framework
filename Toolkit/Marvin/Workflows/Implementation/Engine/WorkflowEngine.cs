using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Marvin.Workflows
{
    internal class WorkflowEngine : IWorkflowEngine
    {
        public WorkflowEngine()
        {
            State = EngineState.Create(this);
        }

        internal EngineState State { get; set; }

        /// 
        public IWorkflow ExecutedWorkflow { get; private set; }

        ///
        public IWorkplanContext Context { get; set; }

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
            foreach (var endPlace in ExecutedWorkflow.EndPlaces())
            {
                endPlace.TokenAdded += EndPlaceReached;
            }
        }

        private void OnTransitionTriggered(object sender, EventArgs e)
        {
            // ReSharper disable once PossibleNullReferenceException
            TransitionTriggered(this, (IObservableTransition)sender);
        }

        private void EndPlaceReached(object sender, IToken token)
        {
            var place = (IPlace) sender;
            // Raise only if main token reached end
            if (place.Classification == NodeClassification.Failed || token is MainToken)
            {
                State.Completed();
                Completed(this, place);
            }
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

        internal WorkflowSnapshot ExecutePause()
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

            return snapShot;
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

        internal void ExecuteRestore(WorkflowSnapshot snapshot)
        {
            var allHolder = GetAllHolders();
            foreach (var holder in snapshot.Holders)
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
            foreach (var endPlace in ExecutedWorkflow.EndPlaces())
            {
                endPlace.TokenAdded -= EndPlaceReached;
            }
            ExecutedWorkflow = null;
        }
    }
}