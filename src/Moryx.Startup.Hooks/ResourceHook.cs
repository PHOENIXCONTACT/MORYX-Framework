// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Resources;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Startup.Hooks;

public class ResourceHook(IModuleManager moduleManager, IConfiguration configuration, ILogger<ResourceHook> logger)
    : ModuleStartHook<IResourceManagement, ResourceHookConfig>(moduleManager, configuration, ConfigKey, logger)
{
    const string ConfigKey = "Hooks:Resources";

    protected override FunctionResult Initialize(ResourceHookConfig config)
    {

        if (config.Initializers is not { Length: > 0 })
        {
            return FunctionResult.WithError("No initializers defined");
        }

        return base.Initialize(config);
    }

    protected override async Task OnModuleStarted(IServerModule module, IResourceManagement facade, ResourceHookConfig config)
    {
        if (!_firstStart)
        {
            return;
        }

        var wasEmpty = !facade.GetResources<IResource>().Any();
        var initializerRan = false;

        foreach (var initializerConfig in config.Initializers)
        {
            if (initializerConfig.Disabled || (!wasEmpty && initializerConfig.OnlyOnFreshDb))
            {
                continue;
            }

            var configType = ReflectionTool.GetPublicClasses<ResourceInitializerConfig>(t =>
                t.Name == initializerConfig.ConfigType || t.FullName == initializerConfig.ConfigType);
            var match = configType.FirstOrDefault(t => t.FullName == initializerConfig.ConfigType);
            match ??= configType.FirstOrDefault();
            if (match == null)
            {
                // TODO warning
                continue;
            }
            ResourceInitializerConfig? instantiatedConfig;
            if (match.GetConstructor([]) is ConstructorInfo ci)
            {
                instantiatedConfig = ci.Invoke([]) as ResourceInitializerConfig;
                initializerConfig.Parameters?.Bind(instantiatedConfig);
            }
            else
            {
                instantiatedConfig = initializerConfig.Parameters?.Get(match) as ResourceInitializerConfig;
            }

            if (instantiatedConfig == null)
            {
                continue;
            }
            try
            {
                await facade.ExecuteInitializerAsync(instantiatedConfig, CancellationToken.None);
                initializerRan = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to run initializer: {ex.Message}");
            }
        }
        if (initializerRan)
        {
            await ModuleManager.ReincarnateModuleAsync(module);
        }
    }
}
