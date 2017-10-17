using System;
using Marvin.Container;

namespace Marvin.Model
{
    /// <summary>
    /// Registration attribute for data model factories
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ModelAttribute : RegistrationAttribute
    {
        /// <summary>
        /// Register this factory using the models namespace
        /// </summary>
        /// <param name="services">Namespace of the model</param>
        public ModelAttribute(params Type[] services) : base(LifeCycle.Singleton, services)
        {
        }
    }
}
