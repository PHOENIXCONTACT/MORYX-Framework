using System.Collections.Generic;
using Marvin.Workflows.Transitions;

namespace Marvin.Workflows
{
    /// <summary>
    /// Context the workplan is executed on
    /// </summary>
    public interface IWorkplanContext
    {
        /// <summary>
        /// Check if a step was disabled
        /// </summary>
        bool IsDisabled(long stepId);
    }
}