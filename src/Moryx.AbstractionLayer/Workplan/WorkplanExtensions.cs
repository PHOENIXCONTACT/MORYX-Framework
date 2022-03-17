using System;
using System.ComponentModel.DataAnnotations;
using Moryx.Workflows;
using Moryx.Workflows.Validation;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Extensions for workplan objects
    /// </summary>
    public static class WorkplanExtensions
    {
        /// <summary>
        /// Adds a step to the workplan
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <param name="task">The task to add.</param>
        /// <param name="parameter">The parameter for the task.</param>
        /// <param name="input">The input for the step</param>
        /// <param name="outputs">The outputs of the steps.</param>
        /// <returns></returns>
        public static TaskStep<TActivity, TParam> AddStep<TActivity,TParam>(this Workplan workplan, TaskStep<TActivity,TParam> task, TParam parameter, IConnector input, params IConnector[] outputs)
            where TActivity : IActivity<TParam>, new()
            where TParam : IParameters, new()
        {
            task.Parameters = parameter;
            task.Inputs[0] = input;

            for (var i = 0; i < outputs.Length; i++)
            {
                task.Outputs[i] = outputs[i];
            }

            if (task.Outputs.Length != outputs.Length)
                throw new ArgumentException($"Wrong number of outputs for the task {task.Name}");

            workplan.Add(task);

            return task;
        }

        /// <summary>
        /// Adds a step to the workplan
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <param name="task">The task to add.</param>
        /// <param name="parameter">The parameter for the task.</param>
        /// <param name="input">The input for the step</param>
        /// <param name="outputs">The outputs of the steps.</param>
        /// <returns></returns>
        public static TaskStep<TActivity, TProcParam, TParam> AddStep<TActivity, TProcParam, TParam>(this Workplan workplan, TaskStep<TActivity, TProcParam, TParam> task, TParam parameter, IConnector input, params IConnector[] outputs)
            where TActivity : IActivity<TProcParam>, new()
            where TProcParam : IParameters
            where TParam : TProcParam, new()
        {
            task.Parameters = parameter;
            task.Inputs[0] = input;

            for (var i = 0; i < outputs.Length; i++)
            {
                task.Outputs[i] = outputs[i];
            }

            if (task.Outputs.Length != outputs.Length)
                throw new ArgumentException($"Wrong number of outputs for the task {task.Name}");

            workplan.Add(task);

            return task;
        }

        /// <summary>
        /// Adds a step to the workplan
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <param name="task">The task to add.</param>
        /// <param name="parameter">The parameter for the task.</param>
        /// <param name="input">The input for the step</param>
        /// <param name="outputs">The outputs of the steps.</param>
        /// <returns></returns>
        public static TaskStep<TActivity, TParam> AddStep<TActivity, TParam>(this Workplan workplan, TaskStep<TActivity, TParam> task, TParam parameter, IConnector[] input, params IConnector[] outputs)
            where TActivity : IActivity<TParam>, new()
            where TParam : IParameters, new()
        {
            task.Parameters = parameter;
            for (var i = 0; i < input.Length; i++)
            {
                task.Inputs[i] = input[i];
            }

            for (var i = 0; i < outputs.Length; i++)
            {
                task.Outputs[i] = outputs[i];
            }

            workplan.Add(task);

            return task;
        }

        /// <summary>
        /// Adds a step to the workplan
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <param name="task">The task to add.</param>
        /// <param name="parameter">The parameter for the task.</param>
        /// <param name="input">The input for the step</param>
        /// <param name="outputs">The outputs of the steps.</param>
        /// <returns></returns>
        public static TaskStep<TActivity, TProcParam, TParam> AddStep<TActivity, TProcParam, TParam>(this Workplan workplan, TaskStep<TActivity, TProcParam, TParam> task, TParam parameter, IConnector[] input, params IConnector[] outputs)
            where TActivity : IActivity<TProcParam>, new()
            where TProcParam : IParameters
            where TParam : TProcParam, new()
        {
            task.Parameters = parameter;
            for (var i = 0; i < input.Length; i++)
            {
                task.Inputs[i] = input[i];
            }

            for (var i = 0; i < outputs.Length; i++)
            {
                task.Outputs[i] = outputs[i];
            }

            workplan.Add(task);

            return task;
        }

        /// <summary>
        /// Adds a connector to the workplan.
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <param name="name">The name of the connector.</param>
        /// <param name="classification">The classification of the connector.</param>
        /// <returns></returns>
        public static IConnector AddConnector(this Workplan workplan, string name, NodeClassification classification)
        {
            var connector = Workflow.CreateConnector(name, classification);
            workplan.Add(connector);
            return connector;
        }

        /// <summary>
        /// Adds a intermediate connector to the workplan.
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <param name="name">The name of the connector.</param>
        /// <returns></returns>
        public static IConnector AddConnector(this Workplan workplan, string name)
        {
            return workplan.AddConnector(name, NodeClassification.Intermediate);
        }

        /// <summary>
        /// Validates the specified workplan.
        /// </summary>
        /// <param name="workplan">The workplan.</param>
        /// <exception cref="ValidationException">
        /// Error during 'DeadEnd'-Validation
        /// or
        /// Error during 'InfiniteLoop'-Validation
        /// or
        /// Error during 'LoneWolf'-Validation
        /// or
        /// Error during 'LuckStreak'-Validation
        /// </exception>
        public static void Validate(this IWorkplan workplan)
        {
            const ValidationAspect aspects = ValidationAspect.DeadEnd | ValidationAspect.InfiniteLoop | ValidationAspect.LoneWolf | ValidationAspect.LuckyStreak;
            var result = Workflow.Validate(workplan, aspects);

            if (result.Success)
                return;

            foreach (var error in result.Errors)
            {
                throw new ValidationException(error.Print(workplan));
            }
        }
    }
}
