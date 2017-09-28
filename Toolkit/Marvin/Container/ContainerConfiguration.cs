using System;
using System.Collections;
using System.Collections.Generic;

namespace Marvin.Container
{
    /// <summary>
    /// Configuration class passed to the container to include external modifcations
    /// </summary>
    public class ContainerConfiguration
    {
        /// <summary>
        /// Create a new instance of the <see cref="ContainerConfiguration"/>
        /// </summary>
        public ContainerConfiguration()
        {
            Interceptors = new List<InterceptorConfig>();
            Strategies = new List<StrategyConfiguration>();
        }

        /// <summary>
        /// Flag if the container should load interceptors
        /// </summary>
        public bool LoadInterceptors { get; set; }

        /// <summary>
        /// List of configured interceptors
        /// </summary>
        public IList<InterceptorConfig> Interceptors { get; private set; } 

        /// <summary>
        /// List of configured Strategies
        /// </summary>
        public IList<StrategyConfiguration> Strategies { get; private set; } 
    }

    /// <summary>
    /// Confiruation for an interceptor
    /// </summary>
    public class InterceptorConfig
    {
        /// <summary>
        /// Flag if interceptor is enabled
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Name of the interceptor
        /// </summary>
        public string InterceptorName { get; set; }

        /// <summary>
        /// Typ of the interceptor class
        /// </summary>
        public Type InterceptorType { get; set; }

        /// <summary>
        /// Delegate with first object as Kernel
        /// </summary>
        public Action<object, IDictionary> DynamicParameters { get; set; }
    }

    /// <summary>
    /// Configuration used to configure the correct strategy implementation
    /// </summary>
    public class StrategyConfiguration
    {
        /// <summary>
        /// Strategy interface
        /// </summary>
        public Type Strategy { get; set; }

        /// <summary>
        /// Config instance of the implementation
        /// </summary>
        public string PluginName { get; set; }
    }
}
