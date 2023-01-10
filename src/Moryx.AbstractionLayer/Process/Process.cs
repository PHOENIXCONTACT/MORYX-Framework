// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moryx.AbstractionLayer.Recipes;

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Base implementation of <see cref="IProcess"/>
    /// </summary>
    public class Process : IProcess
    {
        private readonly List<IActivity> _activities = new List<IActivity>();
        private readonly ReaderWriterLockSlim _activitiesLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        /// <inheritdoc />
        public long Id { get; set; }

        /// <inheritdoc />
        public IRecipe Recipe { get; set; }

        /// <inheritdoc />
        public IEnumerable<IActivity> GetActivities()
        {
            _activitiesLock.EnterReadLock();

            var result = GetActivities(null);

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public IEnumerable<IActivity> GetActivities(Func<IActivity, bool> predicate)
        {
            _activitiesLock.EnterReadLock();

            IActivity []result;

            if (predicate == null)
                result = _activities.ToArray();
            else
                result = _activities.Where(predicate).ToArray();

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public IActivity GetActivity(ActivitySelectionType selectionType)
        {
            _activitiesLock.EnterReadLock();

            var result = GetActivity(selectionType, null);

            _activitiesLock.ExitReadLock();

            return result;
        }

        /// <inheritdoc />
        public IActivity GetActivity(ActivitySelectionType selectionType, Func<IActivity, bool> predicate)
        {
            _activitiesLock.EnterReadLock();

            IActivity result = null;

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
        public void AddActivity(IActivity toAdd)
        {
            _activitiesLock.EnterWriteLock();

            _activities.Add(toAdd);

            _activitiesLock.ExitWriteLock();
        }

        /// <inheritdoc />
        public void RemoveActivity(IActivity toRemove)
        {
            _activitiesLock.EnterWriteLock();

            _activities.Remove(toRemove);

            _activitiesLock.ExitWriteLock();
        }
    }
}
