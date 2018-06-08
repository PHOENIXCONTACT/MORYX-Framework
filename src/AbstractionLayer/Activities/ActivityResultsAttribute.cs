using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Class to decorate activities to declare the results enum
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ActivityResultsAttribute : Attribute
    {
        /// <summary>
        /// Declare all possible results for this activity
        /// </summary>
        public ActivityResultsAttribute(Type resultEnum)
        {
            ResultEnum = resultEnum;
        }

        /// <summary>
        /// Type of the enum containing the results
        /// </summary>
        public Type ResultEnum{ get; private set; }
    }
}