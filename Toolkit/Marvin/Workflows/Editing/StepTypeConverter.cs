using System;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Workflows
{
    /// <summary>
    /// Helper class to group different types of steps into classifications
    /// </summary>
    internal class StepTypeConverter
    {
        /// <summary>
        /// Detemine classification group for step
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static StepClassification ToClassification(Type type)
        {
            // Everything not explicitly set is an execution step
            var classification = StepClassification.Execution;

            // Split and join are recognized explicitly 
            if(type == typeof(SplitWorkplanStep) || type == typeof(JoinWorkplanStep))
                classification = StepClassification.ControlFlow;

            // And subworkpans are another exception
            else if(typeof(ISubworkplanStep).IsAssignableFrom(type))
                classification = StepClassification.Subworkplan;

            return classification;
        }
    }
}