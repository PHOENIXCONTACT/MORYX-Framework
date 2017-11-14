using System.Collections.Generic;
using System.Linq;

namespace Marvin.Workflows.Validation
{
    internal class DeadEndValidator : IWorkplanValidator
    {
        /// <summary>
        /// Aspect this validator covers
        /// </summary>
        public ValidationAspect TargetAspect { get { return ValidationAspect.DeadEnd; } }

        /// <summary>
        /// Validate the given workplan
        /// </summary>
        public bool Validate(IWorkplan workplan, ICollection<ValidationError> errors)
        {
            foreach (var connector in workplan.Connectors)
            {
                // Find a step using the connector as input
                if(workplan.Steps.Any(step => step.Inputs.Contains(connector)))
                    continue;

                //  or if it is the end connector
                if(connector.Classification.HasFlag(NodeClassification.Exit))
                    continue;

                errors.Add(new DeadEndValidationError(connector.Id));
            }
            return errors.Count == 0;
        }
    }

    internal class DeadEndValidationError : ValidationError
    {
        /// <summary>
        /// Create error instance with position id
        /// </summary>
        /// <param name="positionId"></param>
        public DeadEndValidationError(long positionId) : base(positionId)
        {
        }

        /// <summary>
        /// Print error in readable format
        /// </summary>
        public override string Print(IWorkplan workplan)
        {
            var connector = workplan.Connectors.First(c => c.Id == PositionId);
            return string.Format("Connector {0} at postion {1} is not used as an input in any step!", connector.Name, PositionId);
        }
    }
}