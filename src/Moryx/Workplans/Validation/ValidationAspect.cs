// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Workplans.Validation;

/// <summary>
/// Aspect that should be validated
/// </summary>
[Flags]
public enum ValidationAspect
{
    /// <summary>
    /// Detect any places that do not serve as an input for at least one transition
    /// </summary>
    DeadEnd = 1,

    /// <summary>
    /// Find any constellation of transitions and places that will result in an infinite loop
    /// </summary>
    InfiniteLoop = 2,

    /// <summary>
    /// Make sure their is a consistent path of success places through the workplan
    /// </summary>
    LuckyStreak = 4,

    /// <summary>
    /// Find any steps that is not linked to any other step and is unreachable
    /// </summary>
    LoneWolf = 8
}