// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Validation;

/// <summary>
/// Engine validation class
/// </summary>
internal static class WorkplanValidation
{
    /// <summary>
    /// All available validators
    /// </summary>
    private static readonly IWorkplanValidator[] Validators =
    [
        new DeadEndValidator(),
        new LoneWolfValidator()
    ];

    /// <summary>
    /// Validate the workplan under different aspects. Aspects can be combined using '|' operator.
    /// </summary>
    /// <param name="workplan">Workplan to validate</param>
    /// <param name="aspects">Enum flag aspects to validate</param>
    /// <returns><remarks>True</remarks> if validation succeeded. Otherwise <remarks>false</remarks>.</returns>
    public static ValidationResult Validate(IWorkplan workplan, ValidationAspect aspects)
    {
        var valid = true;
        var errors = new List<ValidationError>();

        foreach (var validator in Validators)
        {
            if (aspects.HasFlag(validator.TargetAspect))
                valid &= validator.Validate(workplan, errors);
        }

        return new ValidationResult
        {
            Success = valid,
            Errors = errors.ToArray()
        };
    }
}