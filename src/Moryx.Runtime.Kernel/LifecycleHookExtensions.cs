// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.Tools;

namespace Moryx.Runtime.Kernel;

/// <summary>
/// Extensions to make registering and running lifecycle hooks more convenient
/// </summary>
public static class LifecycleHookExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Finds all lifecycle hooks using Reflection and registers them to the service collection
        /// </summary>
        public IServiceCollection AddMoryxLifecycleHooks()
        {
            var hooks = ReflectionTool.GetPublicClasses<ILifecycleHook>();
            foreach (var hook in hooks)
            {
                services.AddSingleton(typeof(ILifecycleHook), hook);
            }
            return services;
        }
    }

    extension(IServiceProvider provider)
    {
        /// <summary>
        /// Runs all registered lifecycle hooks, ordered by their priority
        /// </summary>
        public async Task RunMoryxLifecycleHooksAsync(CancellationToken cancellationToken = default)
        {
            var hooks = provider.GetServices<ILifecycleHook>().ToArray();

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
