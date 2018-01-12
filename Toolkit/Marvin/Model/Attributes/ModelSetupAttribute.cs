using System;
using Marvin.Container;

namespace Marvin.Model
{
    /// <summary>
    /// Attribute for IModelSetups to determine their target model
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModelSetupAttribute : ModelAttribute
    {
        /// <summary>
        /// Constructor used if the <see cref="IModelSetup"/> is only available for the defined model
        /// </summary>
        public ModelSetupAttribute(string targetModel) : base(targetModel, typeof(IModelSetup))
        {
        }
    }

    /// <summary>
    /// Attribute for IModelSetups to determine their target model
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ModelScriptAttribute : ModelAttribute
    {
        /// <summary>
        /// Constructor used if the <see cref="IModelScript"/> is only available for the defined model
        /// </summary>
        public ModelScriptAttribute(string targetModel) : base(targetModel, typeof(IModelScript))
        {
        }
    }
}