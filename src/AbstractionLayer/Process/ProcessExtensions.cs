namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Extensions for <see cref="IProcess"/>
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// Prepared activity that will be dispatched as soon as a ready to work was send.
        /// </summary>
        public static IActivity NextActivity(this IProcess process)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.ResourceId == 0);
        }

        /// <summary>
        /// Current running activity
        /// </summary>
        public static IActivity CurrentActivity(this IProcess process)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.ResourceId != 0 && activity.Result == null);
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
            return process.GetActivity(ActivitySelectionType.LastOrDefault, a => a.Type == typeName);
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