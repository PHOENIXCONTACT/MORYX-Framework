// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel;
using Moryx.Modules;

namespace Moryx.Container
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
        /// <returns>Name of interface implementation</returns>
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
        protected override Func<IKernelInternal, IReleasePolicy, object> BuildFactoryComponent(MethodInfo method, string componentName, Type componentType, Arguments additionalArguments)
        {
            var createFunc = new Func<IKernelInternal, IReleasePolicy, object>((kernel, policy) =>
            {
                var instance = base.BuildFactoryComponent(method, componentName, componentType, additionalArguments)(kernel, policy);

                var config = additionalArguments["config"];
                var configType = config.GetType();

                var componentInterfaces = componentType.GetInterfaces();

                // Invoke Initialize for IConfiguredInitializable<>
                var isConfiguredInitializable = componentInterfaces
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConfiguredInitializable<>));

                if (isConfiguredInitializable)
                {
                    var genericPluginApi = typeof(IConfiguredInitializable<>).MakeGenericType(configType);

                    var initMethod = genericPluginApi.GetMethod(nameof(IConfiguredInitializable<>.Initialize),
                        [configType]);
                    initMethod!.Invoke(instance, [config]);
                    return instance;
                }

                // Invoke Initialize for IAsyncConfiguredInitializable<>
                var isAsyncConfiguredInitializable = componentInterfaces
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncConfiguredInitializable<>));

                if (isAsyncConfiguredInitializable)
                {
                    var genericPluginApi = typeof(IAsyncConfiguredInitializable<>).MakeGenericType(configType);

                    var initMethod = genericPluginApi.GetMethod(nameof(IAsyncConfiguredInitializable<>.InitializeAsync),
                        [configType]);

                    var task = (Task)initMethod!.Invoke(instance, [config])!;
                    task.GetAwaiter().GetResult();

                    return instance;
                }

                return instance;
            });

            return createFunc;
        }
    }
}
