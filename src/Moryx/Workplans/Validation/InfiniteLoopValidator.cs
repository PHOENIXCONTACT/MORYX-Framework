// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Detects steps trapped in cycles with no path to an exit connector.
/// </summary>
internal class InfiniteLoopValidator : IWorkplanValidator
{
    /// <summary>
    /// Aspect this validator covers
    /// </summary>
    public ValidationAspect TargetAspect => ValidationAspect.InfiniteLoop;

    /// <summary>
    /// Validate the given workplan
    /// </summary>
    public bool Validate(IWorkplan workplan, ICollection<ValidationError> errors)
    {
        // Backward reachability: find all connectors from which an exit can be reached
        var canReachExit = workplan.Connectors
            .Where(c => c.Classification.HasFlag(NodeClassification.Exit))
            .ToHashSet();

        // Propagate backwards through steps: if any output can reach exit, all inputs can too
        bool expanded;
        do
        {
            expanded = false;
            foreach (var step in workplan.Steps)
            {
                if (!step.Outputs.Any(o => o is not null && canReachExit.Contains(o)))
                {
                    continue;
                }

                foreach (var input in step.Inputs)
                {
                    if (canReachExit.Add(input))
                    {
                        expanded = true;
                    }
                }
            }
        } while (expanded);

        // Steps whose non-null outputs all fail to reach an exit are stuck in an infinite loop
        foreach (var step in workplan.Steps)
        {
            var hasOutputs = false;
            var allStuck = true;
            foreach (var output in step.Outputs)
            {
                if (output is null)
                {
                    continue;
                }

                hasOutputs = true;
                if (canReachExit.Contains(output))
                {
                    allStuck = false;
                    break;
                }
            }

            if (hasOutputs && allStuck)
            {
                errors.Add(new InfiniteLoopValidationError(step.Id));
            }
        }

        return errors.Count == 0;
    }
}

internal class InfiniteLoopValidationError : ValidationError
{
    /// <summary>
    /// Create error instance with position id
    /// </summary>
    public InfiniteLoopValidationError(long positionId) : base(positionId)
    {
    }

    /// <summary>
    /// Print error in readable format
    /// </summary>
    public override string Print(IWorkplan workplan)
    {
        var targetStep = workplan.Steps.First(step => step.Id == PositionId);
        return string.Format(CultureInfo.CurrentCulture, Strings.InfiniteLoopValidationError_Message, targetStep.Name, PositionId);
    }
}
