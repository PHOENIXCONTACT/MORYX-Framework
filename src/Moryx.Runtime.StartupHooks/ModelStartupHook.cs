// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Configuration;
using Moryx.Model;

namespace Moryx.Runtime.StartupHooks;

/// <summary>
/// Hook that allows deleting or creating databases on startup
/// </summary>
public sealed class ModelStartupHook(IConfigManager configManager, IDbContextManager dbContextManager, ILogger<ModelStartupHook> logger) : IStartupHook
{
    /// <inheritdoc />
    public int Priority { get; } = configManager.GetConfiguration<ModelStartupHookConfig>().Priority;

    /// <inheritdoc />
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var config = configManager.GetConfiguration<ModelStartupHookConfig>();
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
