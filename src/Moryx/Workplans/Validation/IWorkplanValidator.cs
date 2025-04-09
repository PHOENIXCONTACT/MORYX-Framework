// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Validation
{
    /// <summary>
    /// Interface for all validators
    /// </summary>
    internal interface IWorkplanValidator
    {
        /// <summary>
        /// Aspect this validator covers
        /// </summary>
        ValidationAspect TargetAspect { get; }

        /// <summary>
        /// Validate the given workplan
        /// </summary>
        bool Validate(IWorkplan workplan, ICollection<ValidationError> errors);
    }
}
