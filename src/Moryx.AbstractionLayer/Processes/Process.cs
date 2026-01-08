// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.AbstractionLayer.Processes
{
    /// <summary>
    /// Base implementation of <see cref="IProcess"/>
    /// </summary>
    public class Process : IProcess
    {
        private readonly List<Activity> _activities = [];
        private readonly ReaderWriterLockSlim _activitiesLock = new(LockRecursionPolicy.SupportsRecursion);

        /// <inheritdoc />
        public virtual long Id { get; set; }

        /// <inheritdoc />
        public IRecipe Recipe { get; set; }

        /// <inheritdoc />
        public virtual IEnumerable<Activity> GetActivities()
        {
            _activitiesLock.EnterReadLock();

            var result = GetActivities(null);

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public virtual IEnumerable<Activity> GetActivities(Func<Activity, bool> predicate)
        {
            _activitiesLock.EnterReadLock();

            Activity[] result;

            if (predicate == null)
                result = _activities.ToArray();
            else
                result = _activities.Where(predicate).ToArray();

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public virtual Activity GetActivity(ActivitySelectionType selectionType)
        {
            _activitiesLock.EnterReadLock();

            var result = GetActivity(selectionType, null);

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public virtual Activity GetActivity(ActivitySelectionType selectionType, Func<Activity, bool> predicate)
        {
            _activitiesLock.EnterReadLock();

            Activity result = null;

            var tmpList = predicate != null ? _activities.Where(predicate) : _activities;

            switch (selectionType)
            {
                case ActivitySelectionType.First:
                    result = tmpList.First();
                    break;
                case ActivitySelectionType.FirstOrDefault:
                    result = tmpList.FirstOrDefault();
                    break;
                case ActivitySelectionType.Last:
                    result = tmpList.Last();
                    break;
                case ActivitySelectionType.LastOrDefault:
                    result = tmpList.LastOrDefault();
                    break;
            }

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public virtual void AddActivity(Activity toAdd)
        {
            _activitiesLock.EnterWriteLock();

            _activities.Add(toAdd);

            _activitiesLock.ExitWriteLock();
        }

        /// <inheritdoc />
        public virtual void RemoveActivity(Activity toRemove)
        {
            _activitiesLock.EnterWriteLock();

            _activities.Remove(toRemove);

            _activitiesLock.ExitWriteLock();
        }
    }
}
