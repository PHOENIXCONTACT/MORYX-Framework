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
        /// Calls build on base class and then forwards config via initialize to newly created component
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

                var targetComponentType = componentType;
                var isAsync = componentType.IsGenericType && componentType.GetGenericTypeDefinition() == typeof(Task<>);
                if (isAsync)
                {
                    targetComponentType = componentType.GetGenericArguments()[0];
                }

                var componentInterfaces = targetComponentType.GetInterfaces();

                // Invoke Initialize for IConfiguredInitializable<>
                var isConfiguredInitializable = componentInterfaces
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IConfiguredInitializable<>));

                if (isConfiguredInitializable)
                {
                    ExecuteInitialize(targetComponentType, configType, instance, config, kernel);
                    if (!isAsync)
                        return instance;

                    // Warn if mixing sync initialize with async factory
                    kernel.Logger.WarnFormat(CultureInfo.InvariantCulture,
                        "Component '{0}' with config '{1}' is using " + nameof(IConfiguredInitializable<>) + " but factory uses " + nameof(Task) +
                        ". Ensure the component lifecycle is fully asynchronous to avoid blocking operations or return instance directly without Task.",
                        targetComponentType.FullName, configType.FullName);

                    return ToTypedTask(targetComponentType, Task.FromResult(instance));
                }

                // Invoke InitializeAsync for IAsyncConfiguredInitializable<>
                var isAsyncConfiguredInitializable = componentInterfaces
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncConfiguredInitializable<>));

                if (isAsyncConfiguredInitializable)
                {
                    var initializeTask = ExecuteInitializeAsync(targetComponentType, additionalArguments, configType, instance, config, kernel);

                    if (isAsync)
                    {
                        var t = Task.Run(async () =>
                        {
                            await initializeTask;
                            return instance;
                        });

                        return ToTypedTask(targetComponentType, t);
                    }

                    // Warn if mixing async initialize with sync factory
                    kernel.Logger.WarnFormat(CultureInfo.InvariantCulture,
                        "Component '{0}' with config '{1}' is using " + nameof(IAsyncConfiguredInitializable<>) + " but factory don't uses " +
                        nameof(Task) + ". Ensure the component lifecycle is fully asynchronous to avoid blocking operations or use " +
                        nameof(IConfiguredInitializable<>) + ".",
                        targetComponentType.FullName, configType.FullName);

                    initializeTask.GetAwaiter().GetResult();
                    return instance;
                }

                kernel.Logger.WarnFormat(CultureInfo.InvariantCulture,
                    "Component '{0}' with config '{1}' is not using " + nameof(IConfiguredInitializable<>) +
                    " nor " + nameof(IAsyncConfiguredInitializable<>) + ". Ensure the component lifecycle is properly initialized.",
                    targetComponentType.FullName, configType.FullName);

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
                kernel.Logger.ErrorFormat(e, CultureInfo.InvariantCulture,
                    "Error during initialization of component {0} with config {1}: {2}",
                    componentType.FullName, configType.FullName);
            }
        }

        private static Task ExecuteInitializeAsync(Type componentType, Arguments additionalArguments, Type configType,
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
                return (Task)initMethod!.Invoke(instance, [config, cancellationToken])!;
            }
            catch (Exception e)
            {
                kernel.Logger.ErrorFormat(e, CultureInfo.InvariantCulture,
                    "Error during async initialization of component {0} with config {1}: {2}",
                    componentType.FullName, configType.FullName);
                return Task.FromException(e);
            }
        }

        private static object ToTypedTask(Type targetComponentType, Task<object> originalTask)
        {
            // This allows to cast to a generic Task even if the type is only known at runtime.
            var helperMethod = typeof(TaskHelperClass)
                .GetMethod(nameof(TaskHelperClass.CastTaskResult))!
                .MakeGenericMethod(targetComponentType);

            var typedTask = helperMethod.Invoke(null, [originalTask]);
            return typedTask;
        }

        private class TaskHelperClass
        {
            public static Task<T> CastTaskResult<T>(Task<object> task)
            {
                return Inner();

                async Task<T> Inner()
                {
                    var result = await task.ConfigureAwait(false);
                    return (T)result; // safe cast at runtime
                }
            }
        }
    }
}
