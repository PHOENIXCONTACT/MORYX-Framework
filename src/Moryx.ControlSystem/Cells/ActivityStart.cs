// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.Cells
{
    /// <summary>
    /// Resource message to start an activity in a certain resource
    /// </summary>
    public class ActivityStart : Session
    {
        internal ActivityStart(Session currentSession, Activity activity)
            : base(currentSession)
        {
            Activity = activity;
            // From now on the process becomes the reference
            Reference = ProcessReference.ProcessId(activity.Process.Id);
        }

        /// <summary>
        /// Startable activity created by the production controller
        /// </summary>
        public Activity Activity { get; }

        /// <summary>
        /// Creates the activity result message if the activity already has an result
        /// </summary>
        public ActivityCompleted CreateResult()
        {
            if (Activity.Result == null)
                throw new InvalidOperationException("Can not create completed message for activity without result");

            return new ActivityCompleted(Activity, this);
        }

        /// <summary>
        /// Creates the activity result message if the activity was not completed yet
        /// </summary>
        public ActivityCompleted CreateResult(long result)
        {
            if (Activity.Result == null)
                Activity.Complete(result);

            return new ActivityCompleted(Activity, this);
        }

        /// <summary>
        /// Typed parameters of the activity
        /// </summary>
        public TParameters Parameters<TParameters>() where TParameters : class, IParameters =>
            (TParameters)Activity.Parameters;

        /// <summary>
        /// Typed tracing of the activity
        /// </summary>
        public TTracing Tracing<TTracing>() where TTracing : Tracing, new() =>
            Activity.TransformTracing<TTracing>();

        /// <summary>
        /// Typed recipe of the activity
        /// </summary>
        public TRecipe Recipe<TRecipe>() where TRecipe : class, IRecipe =>
            (TRecipe)Activity.Process.Recipe;
    }
}
