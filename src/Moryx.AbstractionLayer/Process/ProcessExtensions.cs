// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.AbstractionLayer
{
    /// <summary>
    /// Extensions for <see cref="IProcess"/>
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Get one prepared activity that will be dispatched as soon as a ready to work was send.
        /// Mention that, in case of parallel path in a workplan, a process could have multiple prepared activities!
        /// See also: <seealso cref="NextActivities"/>
        /// </summary>
        /// <returns>Last activity of the process that is prepared</returns>
        public static IActivity NextActivity(this IProcess process)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.Tracing?.Started == null);
        }

        /// <summary>
        /// Get all prepared activities that will be dispatched as soon as a ready to work was send.
        /// </summary>
        public static IEnumerable<IActivity> NextActivities(this IProcess process)
        {
            return process.GetActivities(activity => activity.Tracing?.Started == null);
        }

        /// <summary>
        /// Get one of the current running activities of the process. 
        /// Mention that, in case of parallel path in a workplan, a process could have multiple running activities!
        /// See also: <seealso cref="CurrentActivities"/>
        /// </summary>
        /// <returns>Last activity of the process that is running</returns>
        public static IActivity CurrentActivity(this IProcess process)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.Tracing?.Started != null && activity.Result == null);
        }

        /// <summary>
        /// Get all current running activities of the process.
        /// </summary>
        public static IEnumerable<IActivity> CurrentActivities(this IProcess process)
        {
            return process.GetActivities(activity => activity.Tracing?.Started != null && activity.Result == null);
        }

        /// <summary>
        /// Get last completed activity
        /// </summary>
        public static IActivity LastActivity(this IProcess process)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => a.Result != null);
        }

        /// <summary>
        /// Get last activity of a certain type
        /// </summary>
        public static IActivity LastActivity(this IProcess process, string typeName)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => a.GetType().Name == typeName);
        }

        /// <summary>
        /// Gets the last activity of a certain type. Derived types are also considered.
        /// Use <see cref="LastActivity{TActivity}(IProcess, bool)" /> if the exact type is needed.
        /// </summary>
        /// <typeparam name="TActivity">Type of the activity</typeparam>
        /// <param name="process">Extended instance of <see cref="IProcess"/></param>
        public static IActivity LastActivity<TActivity>(this IProcess process) where TActivity : IActivity
        {
            return process.LastActivity<TActivity>(false);
        }

        /// <summary>
        /// Gets the last activity of a certain type.
        /// If exact parameter is set to <c>true</c> only the exact type will be considered.
        /// </summary>
        /// <typeparam name="TActivity">Type of the activity</typeparam>
        /// <param name="process">Extended instance of <see cref="IProcess"/></param>
        /// <param name="exact">If <c>true</c> only the exact type will be considered.</param>
        public static IActivity LastActivity<TActivity>(this IProcess process, bool exact) where TActivity : IActivity
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => !exact && a is TActivity || exact && a.GetType() == typeof(TActivity));
        }
    }
}
