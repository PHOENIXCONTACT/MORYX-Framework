// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Orders.Restrictions;
using Moryx.Users;

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Handles actions on operations which have more dependencies than the operation itself
    /// It is the third pillar between the pool and other components
    /// </summary>
    internal interface IOperationManager
    {
        /// <summary>
        /// Starts the assignment of the operation
        /// </summary>
        void Assign(IOperationData operationData);

        /// <summary>
        /// Returns the begin context of the operation
        /// </summary>
        BeginContext GetBeginContext(IOperationData operationData);

        /// <summary>
        /// Begins the given operation
        /// </summary>
        void Adjust(IOperationData operationData, User user, int amount);

        /// <summary>
        /// Returns the current possible reporting context
        /// </summary>
        ReportContext GetReportContext(IOperationData operationData);

        /// <summary>
        /// Reports the given operation
        /// </summary>
        void Report(IOperationData operationData, OperationReport report);

        /// <summary>
        /// Returns the current possible reporting context
        /// </summary>
        ReportContext GetInterruptContext(IOperationData operationData);

        /// <summary>
        /// Interrupts the given operation
        /// </summary>
        void Interrupt(IOperationData operationData, OperationReport report);

        /// <summary>
        /// Aborts the given operation
        /// </summary>
        void Abort(IOperationData operationData);

        /// <summary>
        /// Event which will be raised when the <see cref="BeginContext"/> of an operation is requested
        /// </summary>
        event EventHandler<BeginRequestEventArgs> BeginRequest;

        /// <summary>
        /// Event which will be raised when the report context of the operation will be requested
        /// </summary>
        event EventHandler<ReportRequestEventArgs> ReportRequest;
    }


    internal class ActionEventArgs<TRestrictionType> : OperationEventArgs where TRestrictionType : IOperationRestriction
    {
        /// <summary>
        /// Restrictions to execute the operation action
        /// </summary>
        public IReadOnlyCollection<TRestrictionType> Restrictions => _restrictions;

        private readonly List<TRestrictionType> _restrictions = new();

        public ActionEventArgs(IOperationData operationData) : base(operationData)
        {
        }

        /// <summary>
        /// Adds restriction to the request
        /// </summary>
        public void AddRestriction(TRestrictionType restriction)
        {
            _restrictions.Add(restriction);
        }
    }

    internal class BeginRequestEventArgs : ActionEventArgs<BeginRestriction>
    {
        public BeginRequestEventArgs(IOperationData operationData) : base(operationData)
        {
        }
    }

    internal class ReportRequestEventArgs : ActionEventArgs<ReportRestriction>
    {
        public ReportType ReportType { get; }

        public ReportRequestEventArgs(IOperationData operationData, ReportType reportType) : base(operationData)
        {
            ReportType = reportType;
        }
    }
}

