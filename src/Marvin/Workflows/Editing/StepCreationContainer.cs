using System;
using System.Linq;
using System.Reflection;
using Marvin.Serialization;
using Marvin.Tools;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.Workflows
{
    /// <summary>
    /// Container representing a concrete workplan step type
    /// </summary>
    internal class StepCreationContainer
    {
        /// <summary>
        /// Type of step that is represented
        /// </summary>
        public Type StepType { get; private set; }

        /// <summary>
        /// Main constructor of the step
        /// </summary>
        public ConstructorInfo MainConstructor { get; private set; }

        /// <summary>
        /// Parameters of the main constructor
        /// </summary>
        public ParameterInfo[] Parameters { get; private set; }

        /// <summary>
        /// Create container for given step type
        /// </summary>
        /// <param name="stepType">Type to create for</param>
        /// <returns>Creation container</returns>
        public static StepCreationContainer FromType(Type stepType)
        {
            var constructors = stepType.GetConstructors();
            if (constructors == null || constructors.Length == 0)
                throw new ArgumentException($"Workplan step type {stepType} does not define a public constructor!", nameof(stepType));

            var ctorAndParams = constructors.Select(constructor => new { constructor, parameters = constructor.GetParameters() }).ToArray();
            var ctor = ctorAndParams.OrderByDescending(cap => cap.parameters.Length).First();

            return new StepCreationContainer
            {
                StepType = stepType,
                MainConstructor = ctor.constructor,
                Parameters = ctor.parameters
            };
        }

        /// <summary>
        /// Export a recipe DTO from this container
        /// </summary>
        /// <param name="index">Index of this container within the cache array</param>
        /// <returns></returns>
        public WorkplanStepRecipe ExportRecipe(int index)
        {
            // Read optional initializer from property
            var properties = EntryConvert.EncodeClass(StepType, EditorVisibleSerialization.Instance);

            // Create the recipe object that can be used for user interfaces to create workplans
            var classification = ToClassification(StepType);
            var recipe = new WorkplanStepRecipe
            {
                Index = index,
                Name = StepType.Name,
                Classification = classification,
                Description = StepType.GetDescription(),
                Properties = properties
            };

            // Encode constructor only for non-subworkplan steps
            if (classification != StepClassification.Subworkplan)
                recipe.Constructor = EntryConvert.EncodeMethod(MainConstructor, EditorVisibleSerialization.Instance);

            return recipe;
        }

        /// <summary>
        /// Instantiate this step with given recipe
        /// </summary>
        /// <param name="recipe">Recipe with all creation values</param>
        /// <param name="workplanSource">Source strategy to load workplan</param>
        /// <returns>New workplan step instance</returns>
        public IWorkplanStep Instantiate(WorkplanStepRecipe recipe, IWorkplanSource workplanSource)
        {
            // Creation uses EntryConvert for constructor invocation unless it is a subworkplanstep
            IWorkplanStep instance;
            if (recipe.Classification == StepClassification.Subworkplan)
            {
                var workplan = workplanSource.Load(recipe.SubworkplanId);
                instance = (IWorkplanStep)MainConstructor.Invoke(new object[] { workplan });
            }
            else
            {
                instance = (IWorkplanStep)EntryConvert.CreateInstance(StepType, recipe.Constructor, EditorVisibleSerialization.Instance);
            }

            // Set properties on the entity
            EntryConvert.UpdateInstance(instance, recipe.Properties);

            return instance;
        }

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
            if (type == typeof(SplitWorkplanStep) || type == typeof(JoinWorkplanStep))
                classification = StepClassification.ControlFlow;

            // And subworkpans are another exception
            else if (typeof(ISubworkplanStep).IsAssignableFrom(type))
                classification = StepClassification.Subworkplan;

            return classification;
        }
    }
}