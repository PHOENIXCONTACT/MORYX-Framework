// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Web.Models;

/// <summary>
/// Response type for workplan validation error 
/// </summary>
public class ValidationErrorModel
{
    /// <inheritdoc cref="ValidationErrorModel.Error" />
    public string Error { get; set; }

    /// <inheritdoc cref="ValidationErrorModel.Position" />
    public long Position { get; set; }
}
