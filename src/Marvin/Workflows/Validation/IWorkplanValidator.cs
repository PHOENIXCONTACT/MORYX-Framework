using System.Collections.Generic;

namespace Marvin.Workflows.Validation
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