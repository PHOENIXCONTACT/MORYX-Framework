// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.ControlSystem.ProcessEngine.Processes;

/// <summary>
/// <see cref="EventArgs"/> for activity changes
/// </summary>
internal class ActivityEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ActivityEventArgs"/> class.
    /// </summary>
    public ActivityEventArgs(ActivityData activityData, ActivityState trigger)
    {
        ActivityData = activityData;
        Trigger = trigger;
    }

    /// <summary>
    /// The changed or added activity data
    /// </summary>
    public ActivityData ActivityData { get; }

    /// <summary>
    /// Trigger state of the activity for executing the event chain
    /// </summary>
    public ActivityState Trigger { get; }
}