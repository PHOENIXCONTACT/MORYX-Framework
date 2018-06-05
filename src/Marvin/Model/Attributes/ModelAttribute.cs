using System;
using Marvin.Container;

namespace Marvin.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModelAttribute : RegistrationAttribute
    {
        /// <summary>
        /// Target model of this registration
        /// </summary>
        public string TargetModel { get; }

        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        public ModelAttribute(string targetModel, params Type[] services) : base(LifeCycle.Singleton, services)
        {
            TargetModel = targetModel;
        }
    }
}
