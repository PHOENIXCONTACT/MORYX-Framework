// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Workflows;
using Moryx.Workflows.Transitions;
using Moryx.Workflows.WorkplanSteps;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Abstract base class of all Tasks
    /// </summary>
    /// <typeparam name="TActivity">Type of the activity object</typeparam>
    /// <typeparam name="TParam">Type of the parameters object</typeparam>
    /// <typeparam name="TProcParam">Intermediate type object for the parameter of the activity</typeparam>
    [DataContract(IsReference = true)]
    public abstract class TaskStep<TActivity, TProcParam, TParam> : WorkplanStepBase, ITaskStep<TParam>, INamedTaskStep
        where TActivity : IActivity<TProcParam>, new()
        where TProcParam : IParameters
        where TParam : TProcParam, new()
    {
        private IIndexResolver _indexResolver;

        /// <summary>
        /// Parameters of this step
        /// </summary>
        [DataMember, EntrySerialize]
        public TParam Parameters { get; set; }

        /// <inheritdoc />
        string INamedTaskStep.Name { get; set; }

        /// <inheritdoc />
        public override string Name => ((INamedTaskStep)this).Name;

        /// <summary>
        /// Instantiate task and provide possible results
        /// </summary>
        protected TaskStep()
        {
            Parameters = new TParam();

            var displayName = GetType().GetDisplayName();
            ((INamedTaskStep)this).Name = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;

            var resultEnum = typeof(TActivity).GetCustomAttribute<ActivityResultsAttribute>().ResultEnum;
            OutputDescriptions = DescriptionsFromEnum(resultEnum);
            Outputs = new IConnector[OutputDescriptions.Length];
        }

        /// <summary>
        /// Create array of output descriptions using the enum as input
        /// </summary>
        /// <param name="enumType">Type of enum to use</param>
        /// <returns>Array of descriptions</returns>
        private static OutputDescription[] DescriptionsFromEnum(Type enumType)
        {
            return (from value in Enum.GetValues(enumType).OfType<object>()
                    let name = value.ToString()
                    let numeric = (int) value
                    select new OutputDescription
                    {
                        Name = name,
                        OutputType = OutputTypeFromEnum(enumType, value, numeric),
                        MappingValue = numeric
                    }).ToArray();
        }

        private static OutputType OutputTypeFromEnum(Type enumType, object value, int numeric)
        {
            var outputType = OutputType.Unknown;

            var memberInfos = enumType.GetMember(Enum.GetName(enumType, value));
            if (memberInfos.Any())
            {
                var outputTypeAttr = memberInfos[0].GetCustomAttribute<OutputTypeAttribute>(false);
                if (outputTypeAttr != null)
                {
                    outputType = outputTypeAttr.OutputType;
                }
                else
                {
                    outputType = numeric == 0 ? OutputType.Success : OutputType.Alternative;
                }

            }

            return outputType;
        }

        /// <summary>
        /// Creates a <see cref="TaskTransition{T}"/> for this activity type
        /// </summary>
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            if (context.IsDisabled(this))
                return new NullTransition();

            // Create transition
            var indexResolver = _indexResolver ?? (_indexResolver = TransitionBase.CreateIndexResolver(OutputDescriptions));
            return new TaskTransition<TActivity>(Parameters, indexResolver)
            {
                Name = Name
            };
        }
    }

    /// <summary>
    /// Abstract base class of all Tasks
    /// </summary>
    /// <typeparam name="TActivity">Type of the activity object</typeparam>
    /// <typeparam name="TParam">Type of the parameters object</typeparam>
    [DataContract(IsReference = true)]
    public abstract class TaskStep<TActivity, TParam> : TaskStep<TActivity, TParam, TParam>
        where TActivity : IActivity<TParam>, new()
        where TParam : IParameters, new()
    {
    }
}
