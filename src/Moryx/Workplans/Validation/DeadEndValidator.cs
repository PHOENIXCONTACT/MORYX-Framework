// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using Moryx.Properties;

namespace Moryx.Workplans.Validation;

/// <summary>
/// Detects connectors that are used as an output but never as an input by any step, causing tokens to get stuck.
/// </summary>
internal class DeadEndValidator : IWorkplanValidator
{
    /// <inheritdoc />
    public ValidationAspect TargetAspect => ValidationAspect.DeadEnd;

    /// <inheritdoc />
    public bool Validate(IWorkplan workplan, ICollection<ValidationError> errors)
    {
        foreach (var connector in workplan.Connectors)
        {
            // Find a step using the connector as input
            if (workplan.Steps.Any(step => step.Inputs.Contains(connector)))
                continue;

            //  or if it is the end connector
            if (connector.Classification.HasFlag(NodeClassification.Exit))
                continue;

            //  or if it is not used as an output anywhere and is not a start connector
            if (!workplan.Steps.Any(step => step.Outputs.Contains(connector)) && !connector.Classification.HasFlag(NodeClassification.Entry))
                continue;

            errors.Add(new DeadEndValidationError(connector.Id));
        }
        return errors.Count == 0;
    }
}

internal class DeadEndValidationError : ValidationError
{
    /// <inheritdoc />
    public DeadEndValidationError(long positionId) : base(positionId)
    {
    }

    /// <inheritdoc />
    public override string Print(IWorkplan workplan)
    {
        var connector = workplan.Connectors.First(c => c.Id == PositionId);
        return string.Format(CultureInfo.CurrentCulture, Strings.DeadEndValidationError_Message, connector.Name, PositionId);
    }
}
