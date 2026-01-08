// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Capabilities;
using Moryx.ControlSystem.Capabilities;
using Moryx.VisualInstructions;

namespace Moryx.ControlSystem.Activities;

/// <summary>
/// Result enum representing the results of process removal activities
/// </summary>
public enum ProcessFixupResult
{
    /// <summary>
    /// Process was removed
    /// </summary>
    [Display(Name = "Ok")]
    Fixed = 0
}

/// <summary>
/// Activity dispatched for broken processes
/// </summary>
[ActivityResults(typeof(ProcessFixupResult))]
public class ProcessFixupActivity : Activity<VisualInstructionParameters>, IMountingActivity
{
    /// <inheritdoc />
    public MountOperation Operation => MountOperation.Unmount;

    /// <inheritdoc />
    public override ProcessRequirement ProcessRequirement => ProcessRequirement.Required;

    /// <inheritdoc />
    public override ICapabilities RequiredCapabilities => new ProcessFixupCapabilities();

    /// <inheritdoc />
    protected override ActivityResult CreateResult(long resultNumber)
    {
        return ActivityResult.Create((ProcessFixupResult)resultNumber);
    }

    /// <inheritdoc />
    protected override ActivityResult CreateFailureResult()
    {
        return ActivityResult.Create(ProcessFixupResult.Fixed);
    }
}