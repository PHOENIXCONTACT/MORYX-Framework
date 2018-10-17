using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Marvin.Serialization;
using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.AbstractionLayer
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
        where TProcParam : IParameters, new()
        where TParam : TProcParam, new()
    {
        private IIndexResolver _indexResolver;

        /// <summary>
        /// Parameters of this step
        /// </summary>
        [DataMember, EditorVisible]
        public TParam Parameters { get; set; }

        /// <summary>
        /// Instantiate task and provide possible results
        /// </summary>
        protected TaskStep()
        {
            Parameters = new TParam();

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
                        Success = numeric == 0,
                        MappingValue = numeric
                    }).ToArray();
        }

        /// <summary>
        /// Creates a <see cref="TaskTransition{T}"/> for this activity type
        /// </summary>
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            if (context.IsDisabled(this))
                return new NullTransition();

            // Create transition
            var processContext = (ProcessContext)context;
            var resourceId = processContext.PreassignedResource(Id);
            var indexResolver = _indexResolver ?? (_indexResolver = TransitionBase.CreateIndexResolver(OutputDescriptions));
            return new TaskTransition<TActivity>(Parameters, indexResolver, resourceId);
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