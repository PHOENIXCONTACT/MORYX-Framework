// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.ControlSystem.Processes
{
    /// <summary>
    /// Base class for process replacements that either represent an empty slot
    /// or unknown physical state
    /// </summary>
    public abstract class ProcessReplacement : IProcess
    {
        /// <summary>
        /// Create process replacement with write protected id
        /// </summary>
        protected ProcessReplacement(long processId)
        {
            IdValue = processId;
        }

        /// <summary>
        /// Read-only value of the process
        /// </summary>
        protected long IdValue { get; }

        /// <inheritdoc />
        public long Id
        {
            get => IdValue;
            set => throw new NotSupportedException("Changing replacement id is not supported!");
        }

        /// <inheritdoc />
        public IRecipe Recipe { get; set; }

        /// <inheritdoc />
        public IEnumerable<IActivity> GetActivities()
        {
            return [];
        }

        /// <inheritdoc />
        public IEnumerable<IActivity> GetActivities(Func<IActivity, bool> predicate)
        {
            return [];
        }

        /// <inheritdoc />
        public IActivity GetActivity(ActivitySelectionType selectionType)
        {
            return null;
        }

        /// <inheritdoc />
        public IActivity GetActivity(ActivitySelectionType selectionType, Func<IActivity, bool> predicate)
        {
            return null;
        }

        /// <inheritdoc />
        public void AddActivity(IActivity toAdd)
        {
        }

        /// <inheritdoc />
        public void RemoveActivity(IActivity toRemove)
        {
        }
    }
}
