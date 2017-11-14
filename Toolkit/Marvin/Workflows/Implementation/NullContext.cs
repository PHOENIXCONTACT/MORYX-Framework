using System.Collections.Generic;
using Marvin.Workflows.Transitions;

namespace Marvin.Workflows
{
    /// <summary>
    /// Null object pattern for the workplan context
    /// </summary>
    public struct NullContext : IWorkplanContext
    {
        /// <see cref="IWorkplanContext"/>
        public bool IsDisabled(long stepId)
        {
            return false;
        }
    }
}