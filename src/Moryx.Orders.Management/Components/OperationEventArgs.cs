using System;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Event args for operation events
    /// </summary>
    internal class OperationEventArgs : EventArgs
    {
        /// <summary>
        /// Operation
        /// </summary>
        public IOperationData OperationData { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OperationEventArgs"/>
        /// </summary>
        public OperationEventArgs(IOperationData operationData)
        {
            OperationData = operationData;
        }
    }
}