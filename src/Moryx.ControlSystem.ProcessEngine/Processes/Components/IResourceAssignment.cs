using System;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.ProcessEngine.Processes
{
    /// <summary>
    /// Pool listener that assigns the target resource to an activity and updates the
    /// state to <see cref="ActivityState.Configured"/>
    /// </summary>
    internal interface IResourceAssignment
    {
        /// <summary>
        /// Capabilities of a managed cell has changed
        /// </summary>
        event EventHandler<ICapabilities> CapabilitiesChanged;
    }
}