// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.ControlSystem.Jobs;
using Moryx.Orders.Management.Model;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Interface to an operation
    /// </summary>
    internal interface IOperationData
    {
        /// <summary>
        /// Public operation for this business object
        /// </summary>
        InternalOperation Operation { get; }

        /// <summary>
        /// Order of this operation
        /// </summary>
        IOrderData OrderData { get; }

        /// <summary>
        /// Unique identifier of this operation
        /// </summary>
        Guid Identifier { get; }

        /// <summary>
        /// <see cref="Orders.Operation.Number"/>
        /// Routed property from <see cref="Operation"/>
        /// </summary>
        string Number { get; }

        /// <summary>
        /// <see cref="Orders.Operation.TotalAmount"/>
        /// Routed property from <see cref="Operation"/>
        /// </summary>
        int TotalAmount { get; }

        /// <summary>
        /// <see cref="Orders.Operation.TargetAmount"/>
        /// Routed property from <see cref="Operation"/>
        /// </summary>
        int TargetAmount { get; }

        /// <summary>
        /// <see cref="Orders.Operation.Product"/>
        /// Routed property from <see cref="Operation"/>
        /// </summary>
        IProductType Product { get; }

        /// <summary>
        /// Module internal representation of the state
        /// </summary>
        IOperationState State { get; }

        /// <summary>
        /// Number of the production order
        /// </summary>
        int SortOrder { get; set; }

        /// <summary>
        /// Current state of the assignment of the master data
        /// </summary>
        OperationAssignState AssignState { get; }

        /// <summary>
        /// Adds a new Job to the jobs of this operation
        /// </summary>
        void AddJob(Job job);

        /// <summary>
        /// Updates the operation source
        /// </summary>
        void UpdateSource(IOperationSource source);

        /// <summary>
        /// Initializes a new operation by a creation context
        /// </summary>
        IOperationData Initialize(OperationCreationContext context, IOrderData orderData, IOperationSource source);

        /// <summary>
        /// Initializes a existing operation by the database entity
        /// </summary>
        IOperationData Initialize(OperationEntity entity, IOrderData orderData);

        /// <summary>
        /// Creates a new operation based on <see cref="IOperationData"/>
        /// </summary>
        void Assign();

        /// <summary>
        /// Will restore the operation. Call Resume after the Restore.
        /// </summary>
        Task Restore();

        /// <summary>
        /// Resumes the Operation after a Restore. Only call it after a restore.
        /// </summary>
        void Resume();

        /// <summary>
        /// I an operation was created but not started, it can be removed from the system
        /// </summary>
        void Abort();

        /// <summary>
        /// Used to set the operation state after the creation is finished.
        /// </summary>
        void AssignCompleted(bool success);

        /// <summary>
        /// Returns the current possible begin information to start the operation
        /// </summary>
        BeginContext GetBeginContext();

        /// <summary>
        /// Adjusts the current target amount by the given <paramref name="amount"/>
        /// by request of the provided <paramref name="user"/>. This can cause the
        /// completion of the currently running jobs and the (re)creation of a new
        /// job with an adjusted amount.
        /// </summary>
        void Adjust(int amount, User user);

        /// <summary>
        /// Returns the current possible reporting context
        /// </summary>
        ReportContext GetReportContext();

        /// <summary>
        /// Returns the current possible advice context
        /// </summary>
        AdviceContext GetAdviceContext();

        /// <summary>
        /// Disables an operation which leads to complete all jobs
        /// </summary>
        void Interrupt(OperationReport report);

        /// <summary>
        /// Will do a report for the operation.
        /// </summary>
        void Report(OperationReport report);

        /// <summary>
        /// Will do an advice for the operation
        /// </summary>
        void Advice(OperationAdvice advice);

        /// <summary>
        /// A job has made progress
        /// </summary>
        /// <param name="job"></param>
        void JobProgressChanged(Job job);

        /// <summary>
        /// Updates an existing job on the operation
        /// </summary>
        void JobStateChanged(JobStateChangedEventArgs args);

        /// <summary>
        /// Updates the product which should be produced within this operation
        /// </summary>
        void AssignProduct(IProductType productType);

        /// <summary>
        /// Updates the whole list of recipes with the given list
        /// </summary>
        void AssignRecipes(IReadOnlyList<IProductRecipe> recipes);

        /// <summary>
        /// Updates the changed recipe on the operation
        /// </summary>
        void RecipeChanged(IProductRecipe productRecipe);

        /// <summary>
        /// Raised if the operations was updated
        /// </summary>
        event EventHandler<OperationEventArgs> Updated;

        /// <summary>
        /// Raised if the operations was aborted
        /// </summary>
        event EventHandler<OperationEventArgs> Aborted;

        /// <summary>
        /// Raised if the operation was started
        /// </summary>
        event EventHandler<StartedEventArgs> Started;

        /// <summary>
        /// Raised if the operation was interrupted
        /// </summary>
        event EventHandler<ReportEventArgs> Interrupted;

        /// <summary>
        /// Raised if the operation was completed
        /// </summary>
        event EventHandler<ReportEventArgs> Completed;

        /// <summary>
        /// Raised if the operation was partially reported
        /// </summary>
        event EventHandler<ReportEventArgs> PartialReport;

        /// <summary>
        /// Raised if there was an amount adviced for an operation
        /// </summary>
        event EventHandler<AdviceEventArgs> Adviced;

        /// <summary>
        /// Raised if the progress was changed
        /// </summary>
        event EventHandler<OperationEventArgs> ProgressChanged;
    }
}

