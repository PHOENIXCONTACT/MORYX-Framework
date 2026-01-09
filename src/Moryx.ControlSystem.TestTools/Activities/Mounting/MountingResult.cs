// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.VisualInstructions;
using System.ComponentModel.DataAnnotations;

namespace Moryx.ControlSystem.TestTools.Activities;

/// <summary>
/// Result enum representing the results of mounting activities
/// </summary>
public enum MountingResult
{
    /// <summary>
    /// Article was mounted on carrier
    /// </summary>
    [EnumInstruction, Display(Name = "Eingelegt")]
    Mounted = 0,

    /// <summary>
    /// The activity could not be started at all.
    /// </summary>
    TechnicalFailure = 2
}