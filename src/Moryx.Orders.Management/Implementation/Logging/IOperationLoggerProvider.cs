namespace Moryx.Orders.Management
{
    /// <summary>
    /// Interface to provide operation depending logging
    /// </summary>
    internal interface IOperationLoggerProvider
    {
        /// <summary>
        /// Gets the creation logger for the operation
        /// </summary>
        IOperationLogger GetLogger(IOperationData operationData);

        /// <summary>
        /// Removes a logger for the given operation
        /// </summary>
        void RemoveLogger(IOperationData operationData);
    }
}