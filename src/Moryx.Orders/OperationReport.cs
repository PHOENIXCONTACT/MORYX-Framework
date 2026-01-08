// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Users;

namespace Moryx.Orders
{
    /// <summary>
    /// Representation of a report for an operation
    /// </summary>
    public class OperationReport : IPersistentObject
    {
        /// <summary>
        /// Creates a new instance of <see cref="OperationReport"/>
        /// </summary>
        public OperationReport(ConfirmationType confirmationType, int successCount, int failureCount, User user)
        {
            ConfirmationType = confirmationType;
            SuccessCount = successCount;
            FailureCount = failureCount;
            ReportedDate = DateTime.Now;
            User = user;
        }

        /// <summary>
        /// Id of the report
        /// </summary>
        long IPersistentObject.Id { get; set; }

        /// <summary>
        /// Type of the report. Reports can be final or partial
        /// </summary>
        public ConfirmationType ConfirmationType { get; set; }

        /// <summary>
        /// Comment for the report
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Reported success count
        /// </summary>
        public int SuccessCount { get; }

        /// <summary>
        /// Reported failure count
        /// </summary>
        public int FailureCount { get; }

        /// <summary>
        /// Date of the report
        /// </summary>
        public DateTime ReportedDate { get; }

        /// <summary>
        /// The user which creates a report
        /// </summary>
        public User User { get; set; }
    }
}