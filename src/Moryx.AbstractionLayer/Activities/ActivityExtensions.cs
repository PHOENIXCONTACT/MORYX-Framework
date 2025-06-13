using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Workplans;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Extensions for <see cref="Activity"/>
    /// </summary>
    public static class ActivityExtensions
    {
        /// <summary>
        /// Get the task of an activity
        /// </summary>
        public static IWorkplanStep GetTask(this Activity activity)
        {
            if (activity.Process.Recipe is WorkplanRecipe workplanRecipe)
            {
                var step = workplanRecipe.Workplan.Steps.FirstOrDefault(s => s.Id == activity.StepId);
                return step;
            }
            return null;
        }
    }
}
