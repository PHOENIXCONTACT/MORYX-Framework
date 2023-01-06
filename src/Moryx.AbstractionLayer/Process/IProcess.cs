// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Defines how activities should be loaded from a <see cref="IProcess"/>
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
    public interface IProcess : IPersistentObject, IConstraintContext
    {
        /// <summary>
        /// A link to the recipe of this process.
        /// </summary>
        IRecipe Recipe { get; set; }

        /// <summary>
        /// The current and the finished activities of this process.
        /// </summary>
        IEnumerable<IActivity> GetActivities();

        /// <summary>
        /// The current and the finished activities of this process filtered by the given function.
        /// </summary>
        IEnumerable<IActivity> GetActivities(Func<IActivity, bool> predicate);

        /// <summary>
        /// Get an activity using the given selection type.
        /// </summary>
        IActivity GetActivity(ActivitySelectionType selectionType);

        /// <summary>
        /// Get an activity using the given selection type that complies to the given predicate.
        /// </summary>
        IActivity GetActivity(ActivitySelectionType selectionType, Func<IActivity, bool> predicate);

        /// <summary>
        /// Add an activity to the list
        /// </summary>
        void AddActivity(IActivity toAdd);

        /// <summary>
        /// Remove an activity from the list
        /// </summary>
        void RemoveActivity(IActivity toRemove);
    }
}
