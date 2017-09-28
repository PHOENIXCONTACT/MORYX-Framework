using System;
using System.Collections;
using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Marvin.Modules.ModulePlugins;

namespace Marvin.Container
{
    /// <summary>
    /// Selector that uses the given config object to determine component name and execute initialize.
    /// The Create method must have the first parameter "config" of type T.
    /// </summary>
    internal class ConfigBasedComponentSelector : DefaultTypedFactoryComponentSelector, IConfigBasedComponentSelector
    {
        /// <summary>
        /// Read the components name from the <see cref="IPluginConfig.PluginName"/> property.
        /// </summary>
        /// <param name="method">Method invoked on the factory - normally a create method like Create(T config, ....)</param>
        /// <param name="arguments">Argument array that must have the components config as the first parameter.</param>
        /// <returns>Name of interface implemenation</returns>
        protected override string GetComponentName(MethodInfo method, object[] arguments)
        {
            var config = (IPluginConfig)arguments[0];
            return config.PluginName;
        }

        /// <summary>
        /// Calls build on base class and than forwards config via initialize to newly created component
        /// </summary>
        /// <param name="method">Invoked factory method - e.g. Create</param>
        /// <param name="componentName">Name of component being constructed</param>
        /// <param name="componentType">Type of component being constructed</param>
        /// <param name="additionalArguments">Additional arguments passed. In our case "T config"</param>
        /// <returns></returns>
        protected override Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method, string componentName, Type componentType, IDictionary additionalArguments)
        {
            var createFunc = new Func<IKernelInternal, IReleasePolicy, object>((kernel, policy) =>
            {
                var instance = base.BuildFactoryComponent(method, componentName, componentType, additionalArguments)(kernel, policy);

                // TODO: Create better infrastructure
                var config = additionalArguments["config"];
                var initMethod = instance.GetType().GetMethod("Initialize", new []{ config.GetType() });
                initMethod.Invoke(instance, new[] { config });
                return instance;
            });
            return createFunc;
        }
    }
}
