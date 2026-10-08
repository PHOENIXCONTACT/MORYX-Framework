// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Response type for workplan validation
/// </summary>
public class ValidationResultModel
{
    /// <inheritdoc cref="ValidationResultModel.Success" />
    public bool Success { get; set; }

    /// <inheritdoc cref="ValidationResultModel.Errors" />
    public ValidationErrorModel[] Errors { get; set; }
}
