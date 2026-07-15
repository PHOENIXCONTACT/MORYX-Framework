// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Tools;

namespace Moryx.Model;

/// <summary>
/// Hook that allows deleting or creating databases on startup
/// </summary>
public sealed class ModelLifecycleHook(IConfigManager configManager, IDbContextManager dbContextManager, ILogger<ModelLifecycleHook> logger) : ILifecycleHook
{
    /// <inheritdoc />
    public int Priority { get; } = configManager.GetConfiguration<ModelLifecycleHookConfig>().Priority;

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var config = configManager.GetConfiguration<ModelLifecycleHookConfig>();
        if (config.Disabled)
        {
            return;
        }

        if (config.DeleteAllDbs)
        {
            await dbContextManager.DeleteAllConfiguredDatabasesAsync(logger, cancellationToken);
        }
        else
        {
            foreach (var dbContext in config.DbsToDelete ?? [])
            {
                await dbContextManager.DeleteDatabaseByNameAsync(dbContext, logger, cancellationToken);
            }
        }

        if (config.CreateDbs)
        {
            await dbContextManager.CreateAllConfiguredDatabasesAsync(logger, cancellationToken);
        }
    }
}
