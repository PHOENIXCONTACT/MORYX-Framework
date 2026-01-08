// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Constraints;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.AbstractionLayer.Processes
{
    /// <summary>
    /// Defines how activities should be loaded from a <see cref="Process"/>
    /// </summary>
    public enum ActivitySelectionType
    {
        /// <summary>
        /// Will select the first activity. Throws if no activity is available.
        /// </summary>
        First,

        /// <summary>
        /// Will select the first activity. Returns null if no activity is available.
        /// </summary>
        FirstOrDefault,

        /// <summary>
        /// Will select the last activity. Throws if no activity is available.
        /// </summary>
        Last,

        /// <summary>
        /// Will select the last activity. Returns null if no activity is available.
        /// </summary>
        LastOrDefault
    }

    /// <summary>
    /// A process is a sequence of activities defined and parameterized by a recipe.
    /// All activities created for the process are stored with the process for tracing purposes.
    /// All objects representing a process implement the <see cref="IProcess"/> interface.
    /// </summary>
    public interface IProcess : IPersistentObject
    {
        /// <summary>
        /// A link to the recipe of this process.
        /// </summary>
        IRecipe Recipe { get; set; }

        /// <summary>
        /// The current and the finished activities of this process.
        /// </summary>
        IEnumerable<Activity> GetActivities();

        /// <summary>
        /// The current and the finished activities of this process filtered by the given function.
        /// </summary>
        IEnumerable<Activity> GetActivities(Func<Activity, bool> predicate);

        /// <summary>
        /// Get an activity using the given selection type.
        /// </summary>
        Activity GetActivity(ActivitySelectionType selectionType);

        /// <summary>
        /// Get an activity using the given selection type that complies to the given predicate.
        /// </summary>
        Activity GetActivity(ActivitySelectionType selectionType, Func<Activity, bool> predicate);

        /// <summary>
        /// Add an activity to the list
        /// </summary>
        void AddActivity(Activity toAdd);

        /// <summary>
        /// Remove an activity from the list
        /// </summary>
        void RemoveActivity(Activity toRemove);
    }
}
