using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Marvin.Serialization;
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
            // Read mandatory initializers from constructor
            var constructorParameters = new Entry
            {
                Key = new EntryKey
                {
                    Name = "ConstructorParameters"
                },
                SubEntries = Parameters.Select(WorkplanStepHelper.FromParameter).ToList()
            };

            // Read optional initializer from property
            var properties = GetProperties(StepType);

            // Create the recipe object that can be used for user interfaces to create workplans
            var desAtt = StepType.GetCustomAttribute<DescriptionAttribute>();
            var recipe = new WorkplanStepRecipe
            {
                Index = index,
                Name = StepType.Name,
                Classification = StepTypeConverter.ToClassification(StepType),
                Description = desAtt == null ? string.Empty : desAtt.Description,
                ConstructorParameters = constructorParameters,
                Properties = properties
            };

            return recipe;
        }

        /// <summary>
        /// Get properties of this step type in converted form
        /// </summary>
        public static Entry GetProperties(Type stepType, object instance = null)
        {
            // Encode step into Entry format
            var initializer = instance == null
                ? EntryConvert.EncodeClass(stepType, WorkplanSerialization.Simple)
                : EntryConvert.EncodeObject(instance, WorkplanSerialization.Simple);

            var entry = new Entry
            {
                Key = new EntryKey
                {
                    Name = stepType.Name
                },
                SubEntries = initializer.SubEntries.ToList()
            };

            // Check if any workplans are referenced
            var workplan = stepType.GetProperties().FirstOrDefault(WorkplanSerialization.IsWorkplanReference);
            if (workplan == null)
                return entry;

            // Append workplan property to initializers
            initializer = WorkplanStepHelper.FromWorkplanProperty(workplan, instance);
            entry.SubEntries = initializer.SubEntries.ToList();

            return entry;
        }

        /// <summary>
        /// Instantiate this step with given recipe
        /// </summary>
        /// <param name="recipe">Recipe with all creation values</param>
        /// <param name="workplanSource">Source strategy to load workplan</param>
        /// <returns>New workplan step instance</returns>
        public IWorkplanStep Instantiate(WorkplanStepRecipe recipe, IWorkplanSource workplanSource)
        {
            // Prepare object array of arguments
            var arguments = (from param in Parameters
                             let initializer = recipe.ConstructorParameters.SubEntries.First(i => i.Key.Identifier == param.Name)
                             select initializer.Value.Type == EntryValueType.Int64 // ToDo: HowTo detect a workplan correctly?
                                ? workplanSource.Load(int.Parse(initializer.Value.Current)) ?? ToObject(param.ParameterType, initializer)
                                : ToObject(param.ParameterType, initializer)).ToArray();

            // Create instance
            var instance = MainConstructor.Invoke(arguments);

            // Update
            var serialization = new WorkplanSerialization(workplanSource);
            EntryConvert.UpdateInstance(instance, new Entry { SubEntries = recipe.Properties.SubEntries.ToList() }, serialization);

            return (IWorkplanStep)instance;
        }

        /// <summary>
        /// Transform an entry back into the value object for a certain type
        /// </summary>
        private static object ToObject(Type type, Entry entry)
        {
            object value = null;
            if (entry.Value.Type == EntryValueType.Class)
            {
                value = EntryConvert.CreateInstance(type, entry);
            }
            else if (entry.Value.Type == EntryValueType.Collection)
            {
                value = WorkplanSerialization.BuildCollection(type, entry);
            }
            else if (entry.Value.Current != null)
            {
                value = EntryConvert.ToObject(type, entry.Value.Current);
            }
            return value;
        }
    }
}