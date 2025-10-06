// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using Moryx.Workplans.Transitions;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Transition representing a certain task
    /// </summary>
    public class TaskTransition<TActivity> : TransitionBase, IObservableTransition, ITaskTransition
        where TActivity : IActivity, new()
    {
        /// <summary>
        /// Flag if engine of this transition is still running
        /// </summary>
        private bool _paused;

        /// <summary>
        /// Parameters for the target activity
        /// </summary>
        private readonly IParameters _parameters;
        /// <summary>
        /// Map of output names to index of the Outputs array
        /// </summary>
        private readonly IIndexResolver _indexResolver;

        /// <summary>
        /// Name of the task
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Create a new instance of the <see cref="TaskTransition{T}"/>
        /// </summary>
        public TaskTransition(IParameters parameters, IIndexResolver resolver)
        {
            _parameters = parameters;
            _indexResolver = resolver;
        }

        ///
        protected override void InputTokenAdded(object sender, IToken token)
        {
            Executing(() => TakeToken((IPlace)sender, token));

            // Raise the triggered event if engine is still running
            if (!_paused)
                Triggered(this, new EventArgs());
        }

        /// <see cref="IObservableTransition"/>
        public event EventHandler Triggered;

        /// <see cref="ITokenHolder.Pause"/>
        public override void Pause()
        {
            _paused = true;
        }

        /// <see cref="ITokenHolder.Resume"/>
        public override void Resume()
        {
            // Only trigger if we did not return from a pause
            if (!_paused)
                Triggered(this, new EventArgs());
            _paused = false;
        }

        /// <summary>
        /// Method called when the activity was completed
        /// </summary>
        /// <param name="result">Result of the execution</param>
        public void Completed(ActivityResult result)
        {
            Executing(delegate
            {
                var outputIndex = _indexResolver.Resolve(result.Numeric);
                PlaceToken(Outputs[outputIndex], StoredTokens.First());
            });
        }

        /// <summary>
        /// Activity created by this task
        /// </summary>
        public Type ActivityType => typeof(TActivity);

        /// <summary>
        /// Create activity instance from transition
        /// </summary>
        /// <returns>the new IActivity object</returns>
        public IActivity CreateActivity(IProcess process)
        {
            // Create activity
            var activity = new TActivity
            {
                Parameters = _parameters.Bind(process),
                Process = process
            };

            // Set name of activity
            if (activity is Activity activityClass)
                activityClass.Name = Name;

            // Link objects
            process.AddActivity(activity);

            return activity;
        }
    }
}
