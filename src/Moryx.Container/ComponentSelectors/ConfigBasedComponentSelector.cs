// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
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
        private const string ConfigParameterName = "config";
        private const string CancellationTokenParameterName = "cancellationToken";

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

                var config = additionalArguments[ConfigParameterName];
                var configType = config.GetType();

                var componentInterfaces = componentType.GetInterfaces();

                // Invoke Initialize for IConfiguredInitializable<>
                var isConfiguredInitializable = componentInterfaces
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConfiguredInitializable<>));

                if (isConfiguredInitializable)
                {
                    ExecuteInitialize(componentType, configType, instance, config, kernel);
                    return instance;
                }

                // Invoke Initialize for IAsyncConfiguredInitializable<>
                var isAsyncConfiguredInitializable = componentInterfaces
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncConfiguredInitializable<>));

                if (isAsyncConfiguredInitializable)
                {
                    ExecuteInitializeAsync(componentType, additionalArguments, configType, instance, config, kernel);
                    return instance;
                }

                return instance;
            });

            return createFunc;
        }

        private static void ExecuteInitialize(Type componentType, Type configType, object instance, object config, IKernelInternal kernel)
        {
            var genericPluginApi = typeof(IConfiguredInitializable<>).MakeGenericType(configType);

            try
            {
                var initMethod = genericPluginApi.GetMethod(nameof(IConfiguredInitializable<>.Initialize),
                    [configType]);
                initMethod!.Invoke(instance, [config]);
            }
            catch (Exception e)
            {
                kernel.Logger.Error(() => string.Format(CultureInfo.InvariantCulture, "Error during async initialization of component {0} with config {1}: {2}",
                    componentType.FullName, configType.FullName, e));
            }
        }

        private static void ExecuteInitializeAsync(Type componentType, Arguments additionalArguments, Type configType,
            object instance, object config, IKernelInternal kernel)
        {
            var genericPluginApi = typeof(IAsyncConfiguredInitializable<>).MakeGenericType(configType);

            var initMethod = genericPluginApi.GetMethod(nameof(IAsyncConfiguredInitializable<>.InitializeAsync),
                [configType, typeof(CancellationToken)]);

            var cancellationToken = CancellationToken.None;
            if (additionalArguments.Contains(CancellationTokenParameterName))
            {
                cancellationToken = (CancellationToken)additionalArguments[CancellationTokenParameterName];
            }

            try
            {
                var task = (Task)initMethod!.Invoke(instance, [config, cancellationToken])!;
                task.GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                kernel.Logger.Error(() => string.Format(CultureInfo.InvariantCulture, "Error during async initialization of component {0} with config {1}: {2}",
                    componentType.FullName, configType.FullName, e));
            }
        }
    }
}
