using System;
using System.Threading;

namespace Marvin.Runtime.Tasks
{
    /// <summary>
    /// Task interface
    /// </summary>
    public interface ITask : IDisposable
    {
        /// <summary>
        /// Representive name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Run or resume this task
        /// </summary>
        void Run(CancellationToken token);

        /// <summary>
        /// Estimate current execution progress in percent
        /// </summary>
        float EstimateProgress();

        /// <summary>
        /// Event indicating that this instance is being disposed by the parent. This must be ra
        /// </summary>
        event EventHandler Disposing;
    }
}
