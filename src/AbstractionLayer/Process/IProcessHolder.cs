using System;
using Marvin.AbstractionLayer.Capabilities;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface for resources that hold a process
    /// </summary>
    public interface IProcessHolder : IPublicResource
    {
        /// <summary>
        /// Process reference held by this holder instance
        /// </summary>
        IProcess Process { get; set; }

        /// <summary>
        /// Event raised when the reference <see cref="Process"/> was changed
        /// </summary>
        event EventHandler<IProcess> ProcessChanged;
    }

    /// <summary>
    /// Capabilities of an <see cref="IProcessHolder"/>
    /// </summary>
    public class ProcessHolderCapabilities : ConcreteCapabilities
    {
        /// <summary>
        /// Id of the process held by the resource
        /// </summary>
        public long ProcessId { get; set; }

        /// <summary>
        /// Check if the given resource holds the process
        /// </summary>
        protected override bool ProvidedBy(ICapabilities provided)
        {
            return (provided as ProcessHolderCapabilities)?.ProcessId == ProcessId;
        }
    }
}