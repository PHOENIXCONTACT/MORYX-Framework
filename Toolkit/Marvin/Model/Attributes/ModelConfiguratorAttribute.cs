using System;

namespace Marvin.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ModelConfiguratorAttribute : ModelAttribute
    {
        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        public ModelConfiguratorAttribute()
            : base(typeof(IModelConfigurator))
        {
        }
    }
}