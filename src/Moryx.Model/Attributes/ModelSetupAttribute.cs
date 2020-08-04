using System;

namespace Moryx.Model
{
    /// <summary>
    /// Registration attribute data model setups
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelSetupAttribute : Attribute
    {
        /// <summary>
        /// Target context of the model setup
        /// </summary>
        public Type TargetContext { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ModelSetupAttribute"/>
        /// </summary>
        public ModelSetupAttribute(Type targetContext)
        {
            TargetContext = targetContext;
        }
    }
}