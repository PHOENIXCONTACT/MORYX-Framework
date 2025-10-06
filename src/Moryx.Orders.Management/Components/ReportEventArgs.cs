// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Event args for reporting events
    /// </summary>
    internal class ReportEventArgs : OperationEventArgs
    {
        /// <summary>
        /// Reporting information for the completed operation
        /// </summary>
        public OperationReport Report { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ReportEventArgs"/>
        /// </summary>
        public ReportEventArgs(IOperationData operationData, OperationReport report)
            : base(operationData)
        {
            Report = report;
        }
    }
}
