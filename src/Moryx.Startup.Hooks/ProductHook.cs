// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moryx.AbstractionLayer.Products;
using Moryx.Runtime.Modules;
using Moryx.Tools;

namespace Moryx.Startup.Hooks;

public class ProductHook(IModuleManager moduleManager, ILogger<ProductHook> logger, IConfiguration configuration)
    : ModuleStartHook<IProductManagement, ProductHookConfig>(moduleManager, configuration, ConfigKey, logger)
{
    const string ConfigKey = "Hooks:Products";

    protected override FunctionResult Initialize(ProductHookConfig config)
    {
        if (config.Importers is not { Length: > 0 })
        {
            return FunctionResult.WithError("No importers defined");
        }

        return base.Initialize(config);
    }
    protected override async Task OnModuleStarted(IServerModule module, IProductManagement facade, ProductHookConfig config)
    {

        var hasEntries = (await facade.LoadTypesAsync(new())).Any();

        foreach (var importerConfig in config.Importers)
        {
            if (importerConfig.Disabled || (hasEntries && importerConfig.OnlyOnFreshDb))
            {
                continue;
            }

            var configType = ReflectionTool.GetPublicClasses(typeof(object), t => t.Name == importerConfig.ConfigType || t.FullName == importerConfig.ConfigType);
            var match = configType.FirstOrDefault(t => t.FullName == importerConfig.ConfigType);
            match ??= configType.FirstOrDefault();
            if (match == null)
            {
                // TODO warning
                continue;
            }
            var imorterParameters = importerConfig.Parameters?.Get(match);
            var result = await facade.ImportAsync(importerConfig.Name, imorterParameters);
        }
    }
}
