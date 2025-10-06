// Copyright (c) 2022, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.Orders.Advice;
using Moryx.Users;

namespace Moryx.Orders
{
    /// <summary>
    /// Order management facade. Declares methods available to manage Orders in the MORYX Platform.
    /// </summary>
    public interface IOrderManagement
    {
        /// <summary>
        /// Returns all current available operations by the given filter
        /// </summary>
        IReadOnlyList<Operation> GetOperations(Func<Operation, bool> filter);

        /// <summary>
        /// Will return the operation with the given identifier
        /// </summary>
        Operation GetOperation(Guid identifier);

        /// <summary>
        /// Will return the operation with the given order and operation numbers
        /// </summary>
        Operation GetOperation(string orderNumber, string operationNumber);

        /// <summary>
        /// Will add a new operation to the pool.
        /// </summary>
        Operation AddOperation(OperationCreationContext context);

        /// <summary>
        /// Will add a new operation to the pool.
        /// </summary>
        Operation AddOperation(OperationCreationContext context, IOperationSource source);

        /// <summary>
        /// Returns a report context of the given operation
        /// </summary>
        BeginContext GetBeginContext(Operation operation);

        /// <summary>
        /// Begins the given operation
        /// </summary>
        void BeginOperation(Operation operation, int amount);

        /// <summary>
        /// Begins the given operation
        /// </summary>
        void BeginOperation(Operation operation, int amount, User user);

        /// <summary>
        /// Aborts the given operation if it was not started before
        /// </summary>
        /// <param name="operation"></param>
        void AbortOperation(Operation operation);

        /// <summary>
        /// Sets the sort order of the given operation
        /// </summary>
        void SetOperationSortOrder(int sortOrder, Operation operation);

        /// <summary>
        /// Returns a report context of the given operation
        /// </summary>
        ReportContext GetReportContext(Operation operation);

        /// <summary>
        /// Processes a report for the given operation
        /// </summary>
        void ReportOperation(Operation operation, OperationReport report);

        /// <summary>
        /// Returns a report context of the given operation to interrupt the operation
        /// </summary>
        ReportContext GetInterruptContext(Operation operation);

        /// <summary>
        /// Processes a interrupt for the given operation
        /// </summary>
        void InterruptOperation(Operation operation, OperationReport report);

        /// <summary>
        /// Updates the operation source
        /// </summary>
        void UpdateSource(IOperationSource source, Operation operation);

        /// <summary>
        /// Will be raised if the progress of an operation was changed
        /// </summary>
        event EventHandler<OperationChangedEventArgs> OperationProgressChanged;

        /// <summary>
        /// Will be raised if an operation was started
        /// </summary>
        event EventHandler<OperationStartedEventArgs> OperationStarted;

        /// <summary>
        /// Will be raised if an operation was closed
        /// </summary>
        event EventHandler<OperationReportEventArgs> OperationCompleted;

        /// <summary>
        /// Will be raised if an operation was interrupted
        /// </summary>
        event EventHandler<OperationReportEventArgs> OperationInterrupted;

        /// <summary>
        /// Will be raised if an operation was partially reported
        /// </summary>
        event EventHandler<OperationReportEventArgs> OperationPartialReport;

        /// <summary>
        /// Will be raised if an operation was adviced
        /// </summary>
        event EventHandler<OperationAdviceEventArgs> OperationAdviced;

        /// <summary>
        /// Will be raised if an operation was changed
        /// </summary>
        event EventHandler<OperationChangedEventArgs> OperationUpdated;

        /// <summary>
        /// Event which will be raised when the begin context of the operation will be requested
        /// </summary>
        event EventHandler<OperationBeginRequestEventArgs> OperationBeginRequest;

        /// <summary>
        /// Event which will be raised when the report context of the operation will be requested
        /// </summary>
        event EventHandler<OperationReportRequestEventArgs> OperationReportRequest;

        /// <summary>
        /// Assigns or updates operation related information like the corresponding product or recipes on the existing operation instance.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> assign.</param>
        void Reload(Operation operation);

        /// <summary>
        /// Returns an <see cref="AdviceContext"/> of the given <paramref name="operation"/> to advice the operation
        /// </summary>
        AdviceContext GetAdviceContext(Operation operation);

        /// <summary>
        /// Tries to advise the <see cref="Operation"/>. 
        /// The returned advice result contains information regarding the successful or unsuccessful attempt.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> to advice.</param>
        /// <param name="advice">The <see cref="OperationAdvice"/> to apply on the <see cref="Operation"/>.</param>
        Task<AdviceResult> TryAdvice(Operation operation, OperationAdvice advice);

        /// <summary>
        /// Returns an array of <see cref="OperationLogMessage"/>s corresponding to the operation.
        /// </summary>
        /// <param name="operation">The <see cref="Operation"/> to retrieve logs for.</param>
        IReadOnlyCollection<OperationLogMessage> GetLogs(Operation operation);

        /// <summary>
        /// Returns a set of recipes this OrderManagement can assign to an Operation corresponding to the <paramref name="identity"/>.
        /// </summary>
        Task<IReadOnlyList<IProductRecipe>> GetAssignableRecipes(ProductIdentity identity);
    }
}
