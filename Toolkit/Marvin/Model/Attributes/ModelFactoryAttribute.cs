using System;
using Marvin.Modules;

namespace Marvin.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ModelFactoryAttribute : ModelAttribute
    {
        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        public ModelFactoryAttribute(string targetModel)
            : base(targetModel, typeof(IUnitOfWorkFactory), typeof(IInitializable))
        {
            Name = targetModel;
        }
    }
}