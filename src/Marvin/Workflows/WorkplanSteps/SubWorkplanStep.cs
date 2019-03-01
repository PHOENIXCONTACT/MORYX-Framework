using System.Linq;
using System.Runtime.Serialization;

namespace Marvin.Workflows.WorkplanSteps
{
    /// <summary>
    /// Base class for all steps that are build around another workplan
    /// </summary>
    [DataContract]
    public abstract class SubworkplanStep : WorkplanStepBase, ISubworkplanStep
    {
        /// <summary>
        /// Create empty instance and set workplan later
        /// </summary>
        protected SubworkplanStep()
        {
        }

        /// <summary>
        /// Create step from another workflow
        /// </summary>
        protected SubworkplanStep(IWorkplan workplan)
        {
            Workplan = workplan;

            // Step outputs are created from all exits of the sub workflow
            OutputDescriptions = (from connector in workplan.Connectors
                                  where connector.Classification.HasFlag(NodeClassification.Exit)
                                  select new OutputDescription
                                  {
                                      Name = connector.Name,
                                      MappingValue = connector.Id,
                                      OutputType = connector.Classification == NodeClassification.End ? OutputType.Success : OutputType.Failure
                                  }).ToArray();
            Outputs = new IConnector[OutputDescriptions.Length];
        }

        /// <see cref="IWorkplanStep"/>
        public override string Name => Workplan.Name;

        [DataMember]
        /// <see cref="ISubworkplanStep"/>
        long ISubworkplanStep.WorkplanId => Workplan.Id;

        /// <summary>
        /// Our subworkplan
        /// </summary>
        protected IWorkplan Workplan { get; private set; }
        /// <see cref="ISubworkplanStep"/>
        IWorkplan ISubworkplanStep.Workplan
        {
            get { return Workplan; }
            set { Workplan = value; }
        }
    }
}