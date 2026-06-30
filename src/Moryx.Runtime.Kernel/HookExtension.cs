// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel;
/// <summary>
/// Extensions to make registering and running startup hooks more convenient
/// </summary>
public static class HookExtension
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Finds startup hooks using Reflection and registers them to the service collection
        /// </summary>
        public IServiceCollection AddStartupHooks()
        {
            var hooks = ReflectionTool.GetPublicClasses<IStartupHook>();
            foreach (var hook in hooks)
            {
                services.AddSingleton(typeof(IStartupHook), hook);
            }
            return services;
        }
    }

    extension(IServiceProvider provider)
    {
        /// <summary>
        /// Runs all registered startup hooks
        /// </summary>
        public async Task RunHooksAsync(CancellationToken cancellationToken)
        {
            var hooks = provider.GetServices<IStartupHook>().ToArray();

            foreach (var hook in hooks
                .OrderBy(h => h.Priority)
                .ThenBy(h => h.GetType().Name))
            {
                cancellationToken.ThrowIfCancellationRequested();
                await hook.RunAsync(cancellationToken);
            }

        }
    }
}
