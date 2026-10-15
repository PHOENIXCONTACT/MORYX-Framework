// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// DTO model of <see cref="Validation.ValidationResult"/>
/// </summary>
public class ValidationResultModel
{
    /// <inheritdoc cref="Validation.ValidationResult.Success" />
    public bool Success { get; set; }

    /// <inheritdoc cref="Validation.ValidationResult.Errors" />
    public ValidationErrorModel[] Errors { get; set; }
}
