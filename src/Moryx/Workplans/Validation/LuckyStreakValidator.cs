// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Validates that there is a consistent path of success outputs from start to a success exit connector.
/// </summary>
internal class LuckyStreakValidator : IWorkplanValidator
{
    /// <summary>
    /// Aspect this validator covers
    /// </summary>
    public ValidationAspect TargetAspect => ValidationAspect.LuckyStreak;

    /// <summary>
    /// Validate the given workplan
    /// </summary>
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
            return errors.Count == 0;
        }

        // Report steps that block the success path: reachable steps with no success output leading forward
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
            }
        }

        // If no specific step was found, report the first start connector
        if (errors.Count == 0)
        {
            var start = workplan.Connectors.FirstOrDefault(c => c.Classification.HasFlag(NodeClassification.Entry));
            if (start is not null)
            {
                errors.Add(new LuckyStreakValidationError(start.Id));
            }
        }

        return errors.Count == 0;
    }
}

internal class LuckyStreakValidationError : ValidationError
{
    /// <summary>
    /// Create error instance with position id
    /// </summary>
    public LuckyStreakValidationError(long positionId) : base(positionId)
    {
    }

    /// <summary>
    /// Print error in readable format
    /// </summary>
    public override string Print(IWorkplan workplan)
    {
        var step = workplan.Steps.FirstOrDefault(s => s.Id == PositionId);
        if (step is not null)
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.LuckyStreakValidationError_NoSuccessOutput, step.Name, PositionId);
        }

        return Strings.LuckyStreakValidationError_NoSuccessPath;
    }
}
