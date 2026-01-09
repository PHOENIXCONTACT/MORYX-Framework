// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Processes;

namespace Moryx.AbstractionLayer.Activities;

/// <summary>
/// Implementation of <see cref="IConstraintContext"/> used during process and activity handling
/// </summary>
public class ActivityConstraintContext : IConstraintContext
{
    /// <summary>
    /// Activity for the constraint to be checked
    /// </summary>
    public Activity Activity { get; }

    /// <summary>
    /// Process for the constraint to be checked
    /// </summary>
    public Process Process => Activity.Process;

    /// <summary>
    /// Creates a new instance of the <see cref="ActivityConstraintContext"/>
    /// </summary>
    public ActivityConstraintContext(Activity activity)
    {
        Activity = activity;
    }
}
