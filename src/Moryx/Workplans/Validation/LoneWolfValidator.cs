// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Validation
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
                if (step.Inputs.Length > 0 && step.Inputs.All(input => workplan.Steps.Any(otherStep => otherStep.Outputs.Contains(input))))
                    continue;

                // If we are linked to the start place we can skip too
                if (step.Inputs.Any(input => input.Classification.HasFlag(NodeClassification.Entry)))
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
        public override string Print(IWorkplan workplan)
        {
            var targetStep = workplan.Steps.First(step => step.Id == PositionId);
            return $"Step {targetStep.Name} at position {PositionId} is unreachable!";
        }
    }
}
