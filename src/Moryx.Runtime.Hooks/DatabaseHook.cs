// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Model;

namespace Moryx.Runtime.Hooks;

/// <inheritdoc />
public sealed class DatabaseHook(IConfigManager configuration, IDbContextManager dbContextManager, ILogger<DatabaseHook> logger) : IStartupHook
{
    public int Priority { get; set; } = configuration.GetConfiguration<DatabaseHookConfig>().Priority;

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
