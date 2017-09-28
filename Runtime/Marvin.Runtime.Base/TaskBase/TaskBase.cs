using System;
using System.Threading;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Base.TaskBase
{
    /// <summary>
    /// Base class for a task.
    /// </summary>
    public abstract class TaskBase : ITask
    {
        /// <summary>
        /// Representive name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Run or resume this task
        /// </summary>
        public abstract void Run(CancellationToken token);

        /// <summary>
        /// Estimate current execution progress in percent
        /// </summary>
        public abstract float EstimateProgress();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (Disposing != null)
                Disposing(this, new EventArgs());
        }

        /// <summary>
        /// Occures when a task is disposed.
        /// </summary>
        public event EventHandler Disposing;
    }
}