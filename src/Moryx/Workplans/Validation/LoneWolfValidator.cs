// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Finds steps that are unreachable because none of their inputs are connected to the output of another step or a start connector.
/// </summary>
internal class LoneWolfValidator : IWorkplanValidator
{
    /// <inheritdoc />
    public ValidationAspect TargetAspect => ValidationAspect.LoneWolf;

    /// <inheritdoc />
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
    /// <inheritdoc />
    public LoneWolfValidationError(long positionId) : base(positionId)
    {
    }

    /// <inheritdoc />
    public override string Print(IWorkplan workplan)
    {
        var targetStep = workplan.Steps.First(step => step.Id == PositionId);
        return string.Format(CultureInfo.CurrentCulture, Strings.LoneWolfValidationError_Message, targetStep.Name, PositionId);
    }
}
