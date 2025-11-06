// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer.Activities
{
    /// <summary>
    /// Extensions for <see cref="Activity"/>
    /// </summary>
    public static class ActivityExtensions
    {
        /// <summary>
        /// Get the task of an <see cref="Activity"/>.
        /// Interface extension for more accessibility and less casting.
        /// </summary>
        /// <param name="activity">Must derive from <seealso cref="Activity"/>!</param>
        public static IWorkplanStep GetTask(this IActivity activity)
        {
            if (activity is not Activity cast)
                throw new ArgumentException("GetTask only works for Activity (IActivity is slightly not enough)!", nameof(activity));

            if (activity.Process.Recipe is WorkplanRecipe workplanRecipe)
            {
                var step = workplanRecipe.Workplan.Steps.FirstOrDefault(s => s.Id == cast.StepId);
                return step;
            }
            return null;
        }
    }
}
