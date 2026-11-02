// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Detects steps trapped in cycles with no path to an exit connector. Uses backward reachability from exit connectors.
/// </summary>
internal class InfiniteLoopValidator : IWorkplanValidator
{
    /// <inheritdoc />
    public ValidationAspect TargetAspect => ValidationAspect.InfiniteLoop;

    /// <inheritdoc />
    public bool Validate(IWorkplan workplan, ICollection<ValidationError> errors)
    {
        var valid = true;

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
                valid = false;
            }
        }

        return valid;
    }
}

internal class InfiniteLoopValidationError : ValidationError
{
    /// <inheritdoc />
    public InfiniteLoopValidationError(long positionId) : base(positionId)
    {
    }

    /// <inheritdoc />
    public override string Print(IWorkplan workplan)
    {
        var targetStep = workplan.Steps.First(step => step.Id == PositionId);
        return string.Format(CultureInfo.CurrentCulture, Strings.InfiniteLoopValidationError_Message, targetStep.Name, PositionId);
    }
}
