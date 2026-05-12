// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Moryx.Tools;

namespace Moryx.Startup.Hooks;

public static class HookExtension
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddStartupHooks()
        {
            var hooks = ReflectionTool.GetPublicClasses<IStartupHook>(h => h.IsClass && !h.IsAbstract);
            foreach (var hook in hooks)
            {
                services.AddSingleton(typeof(IStartupHook), hook);
            }
            return services;
        }
    }
    extension(IServiceProvider provider)
    {
        public async Task RunHooksAsync()
        {
            var hooks = provider.GetServices<IStartupHook>().ToArray();

            foreach (var hook in hooks.OrderBy(h => h.Priority))
            {
                await hook.RunAsync();
            }

        }
    }
}
