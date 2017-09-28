using System;
using System.Threading;
using Marvin.Runtime.Tasks;

namespace Marvin.Runtime.Kernel.Tests.Mocks
{
    /// <summary>
    /// Testtask to test Schedules
    /// </summary>
    internal class TaskMock : ITask
    {
        private string _name = "TestMock";

        /// <summary>
        /// Representive name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Run or resume this task
        /// </summary>
        public void Run(CancellationToken token)
        {
            Executed = true;
            ExecutionCount++;
            ExecutingCallback.Invoke();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (Disposing != null)
                Disposing(this, new EventArgs());
        }

        /// <summary>
        /// Estimate current execution progress in percent
        /// </summary>
        public float EstimateProgress()
        {
            return 0;
        }

        /// <summary>
        /// Occures when a task is disposed.
        /// </summary>
        public event EventHandler Disposing;


        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="TaskMock"/> is executed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if executed; otherwise, <c>false</c>.
        /// </value>
        public bool Executed { get; set; }

        /// <summary>
        /// Gets or sets the execution count.
        /// The number of executions of this task
        /// </summary>
        public int ExecutionCount { get; set; }

        /// <summary>
        /// Gets or sets the executing callback.
        /// Callback is called when ever this task is executed.
        /// </summary>
        public Action ExecutingCallback { get; set; }
    }
}
