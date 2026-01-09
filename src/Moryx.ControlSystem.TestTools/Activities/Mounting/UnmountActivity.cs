// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Capabilities;

namespace Moryx.ControlSystem.TestTools.Activities;

/// <summary>
/// Activity to unmount article from carrier manually
/// </summary>
[ActivityResults(typeof(UnmountingResult))]
public class UnmountActivity : Activity<MountingParameters>, IMountingActivity
{
    /// <inheritdoc />
    public MountOperation Operation => Result.Numeric == (int)UnmountingResult.Removed
        ? MountOperation.Unmount : MountOperation.Unchanged;

    /// <inheritdoc />
    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    /// <inheritdoc />
    public override ICapabilities RequiredCapabilities => new MountCapabilities(false, true);

    /// <summary>
    /// Create a typed result object for this activity based on the result number
    /// </summary>
    protected override ActivityResult CreateResult(long resultNumber)
    {
        return ActivityResult.Create((UnmountingResult)resultNumber);
    }

    /// <summary>
    /// Create a typed result object for a technical failure.
    /// </summary>
    protected override ActivityResult CreateFailureResult()
    {
        return ActivityResult.Create(UnmountingResult.TechnicalFailure);
    }
}