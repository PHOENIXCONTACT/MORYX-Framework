using System;
using Marvin.Testing;

namespace Marvin.Modules.ModulePlugins
{
    /// <summary>
    /// Attribute to decorate a <see cref="IModulePlugin"/> to receive a certain config type
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Class)]
    public class ExpectedConfigAttribute : Attribute
    {
        /// <summary>
        /// Config type expected by this <see cref="IModulePlugin"/>
        /// </summary>
        public Type ExcpectedConfigType { get; private set; }

        /// <summary>
        /// State that this <see cref="IModulePlugin"/> requires config instances of the given type
        /// </summary>
        public ExpectedConfigAttribute(Type configType)
        {
            ExcpectedConfigType = configType;
        }
    }
}
