using System.Linq;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Extensions for <see cref="IProcess"/>
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>Prepared activity that will be dispached as soon as a ready to work was send.</summary>
        public static IActivity NextActivity(this IProcess process)
        {
            return process.GetActivity(ActivitySelectionType.LastOrDefault, activity => activity.ResourceId == 0);
        }

        /// <summary>Current running activity</summary>
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
    }
}