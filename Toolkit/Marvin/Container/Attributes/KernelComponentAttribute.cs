using System;
using System.Linq;
using Marvin.Modules;
using Marvin.Testing;

namespace Marvin.Container
{
    /// <summary>
    /// Register a kernel component
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KernelComponentAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Register a kernel component
        /// </summary>
        public KernelComponentAttribute(params Type[] services)
            : base(LifeCycle.Singleton, services)
        {
        }
    }

    /// <summary>
    /// Register a kernel component
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class InitializableKernelComponentAttribute : KernelComponentAttribute
    {
        /// <summary>
        /// Register a kernel component
        /// </summary>
        public InitializableKernelComponentAttribute(params Type[] services)
            : base(new[] { typeof(IInitializable) }.Union(services).ToArray())
        {
        }
    }

    /// <summary>
    /// Register a kernel plugin
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class KernelPluginAttribute : GlobalComponentAttribute
    {
        /// <summary>
        /// Register a kernel component
        /// </summary>
        public KernelPluginAttribute(params Type[] services)
            : base(LifeCycle.Singleton, services)
        {
        }
    }

}
