// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Response type for workplan <see cref="Validation.ValidationError"/>
/// </summary>
public class ValidationErrorModel
{
    /// <summary>
    /// Readable error message of the validation
    /// </summary>
    public string Error { get; set; }

    /// <inheritdoc cref="Validation.ValidationError.PositionId" />
    public long Position { get; set; }
}
