// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Restrictions;

namespace Moryx.Orders
{
    /// <summary>
    /// Event args for a report request of an operation
    /// </summary>
    public class OperationReportRequestEventArgs : OperationActionRequestEventArgs<ReportRestriction>
    {
        /// <summary>
        /// Specification of the report
        /// </summary>
        public ReportType ReportType { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OperationReportRequestEventArgs"/>
        /// </summary>
        public OperationReportRequestEventArgs(Operation operation, ReportType reportType) : base(operation)
        {
            ReportType = reportType;
        }
    }
}