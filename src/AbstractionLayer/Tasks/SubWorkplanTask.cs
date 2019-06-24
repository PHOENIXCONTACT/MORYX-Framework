using System.Linq;
using System.Runtime.Serialization;
using Marvin.Workflows;
using Marvin.Workflows.Transitions;
using Marvin.Workflows.WorkplanSteps;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// The description of SubWorkplanActivity
    /// </summary>
    [DataContract]
    public abstract class SubWorkplanTask<TParameters> : SubWorkplanStep
        where TParameters : SubWorkplanParameters, new()
    {
        /// <summary>
        /// Empty default constructor to recreate task from database
        /// </summary>
        protected SubWorkplanTask()
        {
        }

        /// <summary>
        /// Create a new subworkplan task for given workplan
        /// </summary>
        /// <param name="workplan"></param>
        protected SubWorkplanTask(IWorkplan workplan)
            : base(workplan)
        {
        }

        private TParameters _parameters;
        private IIndexResolver _indexResolver;
        /// <summary>
        /// Set resource id on transition
        /// </summary>
        protected override TransitionBase Instantiate(IWorkplanContext context)
        {
            if (context.IsDisabled(this))
                return new NullTransition();

            // Create transition
            var processContext = (ProcessContext)context;
            var resourceId = processContext.PreassignedResource(Id);
            // Lazy index resolver creation
            var indexResolver = _indexResolver ?? (_indexResolver = TransitionBase.CreateIndexResolver(OutputDescriptions));
            // Lazy parameters creation
            var parameters = _parameters ?? (_parameters = CreateParameters());

            return new TaskTransition<SubWorkplanActivity>(parameters, indexResolver, resourceId);
        }

        /// <summary>
        /// Create parameters object
        /// </summary>
        protected virtual TParameters CreateParameters()
        {
            var outputs = Workplan.Connectors.Where(ex => ex.Classification.HasFlag(NodeClassification.Exit)).ToArray();
            var success = outputs.FirstOrDefault(output => output.Classification == NodeClassification.End) ?? outputs[0];
            var failure = outputs.FirstOrDefault(output => output.Classification == NodeClassification.Failed) ?? outputs[0];
            return new TParameters
            {
                Workplan = Workplan,
                SuccessResult = success.Id,
                FailureResult = failure.Id
            };
        }
    }
}
