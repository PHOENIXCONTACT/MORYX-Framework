// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Restrictions;

namespace Moryx.Orders
{
    /// <summary>
    /// Report which is not processed.
    /// </summary>
    public class ReportContext : OperationInfo
    {
        /// <summary>
        /// If <c>true</c> the operation can be reported partially
        /// </summary>
        public bool CanPartial { get; set; }

        /// <summary>
        /// If <c>true</c> the operation can be reported finally
        /// </summary>
        public bool CanFinal { get; set; }

        /// <summary>
        /// Currently reported success count
        /// </summary>
        public int ReportedSuccess { get; set; }

        /// <summary>
        /// Currently reported failure count
        /// </summary>
        public int ReportedFailure { get; set; }

        /// <summary>
        /// Value of unreported success parts
        /// </summary>
        public int UnreportedSuccess { get; set; }

        /// <summary>
        /// Value of unreported failure parts
        /// </summary>
        public int UnreportedFailure { get; set; }

        /// <summary>
        /// Reasons why a report is not possible
        /// </summary>
        public RestrictionDescription[] Restrictions { get; set; }
    }
}