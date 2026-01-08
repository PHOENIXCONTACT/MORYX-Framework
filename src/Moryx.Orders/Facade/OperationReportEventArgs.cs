// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders
{
    /// <summary>
    /// Event args of report depending actions on an operation
    /// </summary>
    public class OperationReportEventArgs : OperationChangedEventArgs
    {
        /// <summary>
        /// Reporting information for the operation
        /// </summary>
        public OperationReport Report { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OperationReportEventArgs"/>
        /// </summary>
        public OperationReportEventArgs(Operation operation, OperationReport report)
            : base(operation)
        {
            Report = report;
        }
    }
}