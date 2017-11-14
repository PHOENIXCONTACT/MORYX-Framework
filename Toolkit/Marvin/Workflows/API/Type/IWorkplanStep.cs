using System.Collections.Generic;

namespace Marvin.Workflows
{
    /// <summary>
    /// Single step in the workplan
    /// </summary>
    public interface IWorkplanStep
    {
        /// <summary>
        /// Workplan unique element id of this connector
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Transition name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// All inputs of this step, the amount depends on the workflow
        /// </summary>
        IConnector[] Inputs { get; }

        /// <summary>
        /// All outputs of this step, the amount depends on the type of step
        /// </summary>
        IConnector[] Outputs { get; }

        /// <summary>
        /// Descriptions for each output connector used for result mapping
        /// and visualization
        /// </summary>
        OutputDescription[] OutputDescriptions { get; }

        /// <summary>
        /// Create transistion instance
        /// </summary>
        ITransition CreateInstance(IWorkplanContext context);
    }
}