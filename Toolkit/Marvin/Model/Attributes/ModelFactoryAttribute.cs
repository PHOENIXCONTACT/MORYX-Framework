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
        /// If set, the model is declared as child model 
        /// </summary>
        public string ParentModel { get; }

        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        /// <param name="targetModel">Namespace of the model</param>
        public ModelFactoryAttribute(string targetModel)
            : base(targetModel, typeof(IUnitOfWorkFactory), typeof(IInitializable))
        {
            Name = targetModel;
        }

        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        /// <param name="targetModel">Namespace of the model</param>
        /// <param name="parentModel">If set, the model is declared as child model</param>
        public ModelFactoryAttribute(string targetModel, string parentModel)
            : this(targetModel)
        {
            ParentModel = parentModel;
        }
    }
}