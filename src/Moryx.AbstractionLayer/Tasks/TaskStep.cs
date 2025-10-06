// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using System.Runtime.Serialization;
using Moryx.Serialization;
using Moryx.Tools;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Abstract base class of all Tasks
    /// </summary>
    /// <typeparam name="TActivity">Type of the activity object</typeparam>
    /// <typeparam name="TParam">Type of the parameters object</typeparam>
    /// <typeparam name="TProcParam">Intermediate type object for the parameter of the activity</typeparam>
    [DataContract(IsReference = true)]
    public abstract class TaskStep<TActivity, TProcParam, TParam> : WorkplanStepBase, ITaskStep<TParam>
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

        /// <summary>
        /// Instantiate task and provide possible results
        /// </summary>
        protected TaskStep()
        {
            Parameters = new TParam();

            var displayName = GetType().GetDisplayName();
            Name = string.IsNullOrEmpty(displayName) ? GetType().Name : displayName;

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
                    let name = GetEnumName(value, enumType)
                    let numeric = (int)value
                    select new OutputDescription
                    {
                        Name = name,
                        OutputType = OutputTypeFromEnum(enumType, value, numeric),
                        MappingValue = numeric
                    }).ToArray();
        }

        /// <summary>
        /// Get the enum name. If Display attribute is applied on the enum value
        /// the display value get used otherwise the enum value is used.
        /// </summary>
        /// <param name="value"> value of the enum</param>
        /// <param name="enumType">type of the enum</param>
        /// <returns></returns>
        private static string GetEnumName(object value, Type enumType)
        {
            var enumMembers = enumType.GetMembers();
            var enumValueInfo = enumMembers.FirstOrDefault(x => x.DeclaringType == enumType &&
            x.Name == value.ToString());
            var displayAttributeValue = enumValueInfo.GetDisplayName();

            return displayAttributeValue ?? value.ToString();
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
