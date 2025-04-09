// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Validation
{
    /// <summary>
    /// Result of the workplan validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Indicator wether validation was a success
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Errors found during validation
        /// </summary>
        public ValidationError[] Errors { get; set; }
    }

    /// <summary>
    /// An error that was detected during validation
    /// </summary>
    public abstract class ValidationError
    {
        /// <summary>
        /// Create error instance with position id
        /// </summary>
        /// <param name="positionId"></param>
        protected ValidationError(long positionId)
        {
            PositionId = positionId;
        }

        /// <summary>
        /// Position where the error was detected. May be a place or transition
        /// </summary>
        public long PositionId { get; set; }

        /// <summary>
        /// Print error in readable format
        /// </summary>
        public abstract string Print(IWorkplan workplan);
    }
}
