// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Model;
using Moryx.Runtime;

namespace Moryx.Startup.Hooks;

public class DatabaseHook(IConfigManager configuration, IDbContextManager dbContextManager, ILogger<DatabaseHook> logger) : IStartupHook
{
    public int Priority => 0;

    public async Task RunAsync()
    {
        var config = configuration.GetConfiguration<DatabaseHookConfig>();
        if (config is null || config.Disabled)
        {
            return;
        }
        if (config.DeleteAllDbs)
        {
            await dbContextManager.DeleteAllConfiguredDatabasesAsync(logger, default);
        }
        else
        {
            foreach (var dbContext in config.DbsToDelete ?? [])
            {
                await dbContextManager.DeleteDatabaseByNameAsync(dbContext, logger, default);
            }
        }

        if (config.CreateDbs)
        {
            await dbContextManager.CreateAllConfiguredDatabasesAsync(logger, default);
        }
    }
}
