// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Verifies that a consistent success path exists from the start connector to a success exit connector through <see cref="OutputType.Success"/> outputs only.
/// </summary>
internal class LuckyStreakValidator : IWorkplanValidator
{
    /// <inheritdoc />
    public ValidationAspect TargetAspect => ValidationAspect.LuckyStreak;

    /// <inheritdoc />
    public bool Validate(IWorkplan workplan, ICollection<ValidationError> errors)
    {
        // Forward reachability from start connectors through success outputs only
        var reachable = workplan.Connectors
            .Where(c => c.Classification.HasFlag(NodeClassification.Entry))
            .ToHashSet();

        bool expanded;
        do
        {
            expanded = false;
            foreach (var step in workplan.Steps)
            {
                // Step must be reachable via its inputs
                if (!step.Inputs.Any(reachable.Contains))
                {
                    continue;
                }

                // Only follow success outputs
                for (var i = 0; i < step.Outputs.Length; i++)
                {
                    var output = step.Outputs[i];
                    if (output is null)
                    {
                        continue;
                    }

                    // Check if this output index is a success output
                    if (i < step.OutputDescriptions.Length && step.OutputDescriptions[i].OutputType != OutputType.Success)
                    {
                        continue;
                    }

                    if (reachable.Add(output))
                    {
                        expanded = true;
                    }
                }
            }
        } while (expanded);

        // Check if any success exit connector is reachable
        var successExitReached = workplan.Connectors
            .Any(c => c.Classification == NodeClassification.End && reachable.Contains(c));

        if (successExitReached)
        {
            return true;
        }

        // Report steps that block the success path: reachable steps with no success output leading forward
        var foundBlockingStep = false;
        foreach (var step in workplan.Steps)
        {
            if (!step.Inputs.Any(reachable.Contains))
            {
                continue;
            }

            var hasSuccessOutput = false;
            for (var i = 0; i < step.Outputs.Length; i++)
            {
                if (step.Outputs[i] is null)
                {
                    continue;
                }

                if (i < step.OutputDescriptions.Length && step.OutputDescriptions[i].OutputType != OutputType.Success)
                {
                    continue;
                }

                hasSuccessOutput = true;
                break;
            }

            if (!hasSuccessOutput)
            {
                errors.Add(new LuckyStreakValidationError(step.Id));
                foundBlockingStep = true;
            }
        }

        if (foundBlockingStep)
        {
            return false;
        }

        // No step without success output found — the success path is stuck in a cycle
        foreach (var step in workplan.Steps)
        {
            if (step.Inputs.Any(reachable.Contains))
            {
                errors.Add(new SuccessPathCycleValidationError(step.Id));
            }
        }

        return false;
    }
}

internal class LuckyStreakValidationError : ValidationError
{
    /// <inheritdoc />
    public LuckyStreakValidationError(long positionId) : base(positionId)
    {
    }

    /// <inheritdoc />
    public override string Print(IWorkplan workplan)
    {
        var step = workplan.Steps.First(s => s.Id == PositionId);
        return string.Format(CultureInfo.CurrentCulture, Strings.LuckyStreakValidationError_NoSuccessOutput, step.Name, PositionId);
    }
}

internal class SuccessPathCycleValidationError : ValidationError
{
    /// <inheritdoc />
    public SuccessPathCycleValidationError(long positionId) : base(positionId)
    {
    }

    /// <inheritdoc />
    public override string Print(IWorkplan workplan)
    {
        var step = workplan.Steps.First(s => s.Id == PositionId);
        return string.Format(CultureInfo.CurrentCulture, Strings.SuccessPathCycleValidationError_Message, step.Name, PositionId);
    }
}
