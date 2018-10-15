using System.Collections.Generic;
using System.Linq;

namespace Marvin.Workflows.Validation
{
    internal class LoneWolfValidator : IWorkplanValidator
    {
        /// <summary>
        /// Aspect this validator covers
        /// </summary>
        public ValidationAspect TargetAspect => ValidationAspect.LoneWolf;

        /// <summary>
        /// Validate the given workplan
        /// </summary>
        public bool Validate(IWorkplan workplan, ICollection<ValidationError> errors)
        {
            // Iterate over every step and find all steps without inputs or inputs that are not connected
            foreach (var step in workplan.Steps)
            {
                // Check if the step has inputs that a are used somewhere else as outputs
                if(step.Inputs.Length > 0 && step.Inputs.All(input => workplan.Steps.Any(otherStep => otherStep.Outputs.Contains(input))))
                    continue;

                // If we are linked to the start place we can skip too
                if(step.Inputs.Any(input => input.Classification.HasFlag(NodeClassification.Entry)))
                    continue;

                errors.Add(new LoneWolfValidationError(step.Id));
            }
            return errors.Count == 0;
        }
    }

    internal class LoneWolfValidationError : ValidationError
    {
        /// <summary>
        /// Create error instance with position id
        /// </summary>
        /// <param name="positionId"></param>
        public LoneWolfValidationError(long positionId) : base(positionId)
        {
        }

        /// <summary>
        /// Print error in readable format
        /// </summary>
        // TODO: Does it make sense to pass workplan to error
        public override string Print(IWorkplan workplan)
        {
            var targetStep = workplan.Steps.First(step => step.Id == PositionId);
            return string.Format("Step {0} at position {1} is unreachable!", targetStep.Name, PositionId);
        }
    }
}