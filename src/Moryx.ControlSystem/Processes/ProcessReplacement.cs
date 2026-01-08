// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.Processes;

/// <summary>
/// Base class for process replacements that either represent an empty slot
/// or unknown physical state
/// </summary>
public abstract class ProcessReplacement : Process
{
    private readonly long _processId;

    /// <summary>
    /// Create process replacement with write protected id
    /// </summary>
    protected ProcessReplacement(long processId)
    {
        _processId = processId;
    }

    /// <inheritdoc />
    public override long Id
    {
        get => _processId;
        set => throw new NotSupportedException("Changing replacement id is not supported!");
    }

    /// <inheritdoc />
    public override IEnumerable<Activity> GetActivities()
    {
        return [];
    }

    /// <inheritdoc />
    public override IEnumerable<Activity> GetActivities(Func<Activity, bool> predicate)
    {
        return [];
    }

    /// <inheritdoc />
    public override Activity GetActivity(ActivitySelectionType selectionType)
    {
        return null;
    }

    /// <inheritdoc />
    public override Activity GetActivity(ActivitySelectionType selectionType, Func<Activity, bool> predicate)
    {
        return null;
    }

    /// <inheritdoc />
    public override void AddActivity(Activity toAdd)
    {
    }

    /// <inheritdoc />
    public override void RemoveActivity(Activity toRemove)
    {
    }
}