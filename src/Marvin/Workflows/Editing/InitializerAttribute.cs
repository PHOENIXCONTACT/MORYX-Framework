using System;

namespace Marvin.Workflows
{
    /// <summary>
    /// Attribute used to decorate constructor arguments of workplan steps
    /// in order to serialize them as initializer properties for a workplan step
    /// recipe
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.Constructor)]
    public class InitializerAttribute : Attribute
    {
        /// <summary>
        /// Flag to mark initializers, will use property name as DisplayName
        /// </summary>
        public InitializerAttribute()
        {
        }
        
        /// <summary>
        /// Create initializer with given display name
        /// </summary>
        public InitializerAttribute(string displayName)
        {
            DisplayName = displayName;
        }

        /// <summary>
        /// Name of this parameter later displayed on UI
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Description of this parameter
        /// </summary>
        public string Description { get; set; }
    }
}