using System;
using System.Collections.Generic;
using Moryx.Modules;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// The one and only and mighty and fabulous order pool
    /// </summary>
    internal interface IOperationDataPool : IPlugin
    {
        /// <summary>
        /// Returns all operations which are added to the pool
        /// </summary>
        IReadOnlyList<IOperationData> GetAll();

        /// <summary>
        /// Returns all operation by a specific filter
        /// </summary>
        IReadOnlyList<IOperationData> GetAll(Func<IOperationData, bool> filter);

        /// <summary>
        /// Will return the operation with the given id
        /// </summary>
        IOperationData Get(Guid identifier);

        /// <summary>
        /// Will return the operation with the given order and operation numbers
        /// </summary>
        IOperationData Get(string orderNumber, string operationNumber);

        /// <summary>
        /// Will return the operation data object by the given operation
        /// </summary>
        IOperationData Get(Operation operation);

        /// <summary>
        /// Will return all orders
        /// </summary>
        IReadOnlyList<IOrderData> GetOrders();

        /// <summary>
        /// Will add a new operation to the pool.
        /// </summary>
        IOperationData Add(OperationCreationContext context, IOperationSource source);

        /// <summary>
        /// Raised if an operation was updated
        /// </summary>
        event EventHandler<OperationEventArgs> OperationUpdated;

        /// <summary>
        /// Raised if an operation was aborted
        /// </summary>
        event EventHandler<OperationEventArgs> OperationAborted;

        /// <summary>
        /// Raised if an operation started
        /// </summary>
        event EventHandler<StartedEventArgs> OperationStarted;

        /// <summary>
        /// Raised if an operation was closed
        /// </summary>
        event EventHandler<ReportEventArgs> OperationCompleted;

        /// <summary>
        /// Raised if an operation was interrupted
        /// </summary>
        event EventHandler<ReportEventArgs> OperationInterrupted;

        /// <summary>
        /// Raised if an operation was partially reported
        /// </summary>
        event EventHandler<ReportEventArgs> OperationPartialReport;

        /// <summary>
        /// Raised if an amount was adviced for an operation
        /// </summary>
        event EventHandler<AdviceEventArgs> OperationAdviced;

        /// <summary>
        /// Raised if an operation progress changed
        /// </summary>
        event EventHandler<OperationEventArgs> OperationProgressChanged;
    }
}