// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moryx.Model;

namespace Moryx.Startup.Hooks;

public class DatabaseHook(IConfiguration configuration, IDbContextManager dbContextManager, ILogger<DatabaseHook> logger) : IStartupHook
{
    private const string ConfigKey = "Hooks:Databases";

    public int Priority => 0;

    public async Task RunAsync()
    {
        var config = configuration.GetSection(ConfigKey)?.Get<DatabaseHookConfig>();
        if (config is null || config.Disabled)
        {
            return;
        }

        foreach (var context in dbContextManager.Contexts)
        {
            var configurator = dbContextManager.GetConfigurator(context);
            var testResult = await configurator.TestConnectionAsync(configurator.Config);
            if ((config.DbsToDeleteOnStartup.Contains(context.Name) || config.DeleteAllDbs) && testResult != TestConnectionResult.ConnectionOkDbDoesNotExist)
            {
                await configurator.DeleteDatabaseAsync(configurator.Config);
                testResult = await configurator.TestConnectionAsync(configurator.Config);
            }

            if (testResult is TestConnectionResult.ConnectionOkDbDoesNotExist && config.CreateDbs)
            {
                logger.LogInformation("Db {db} does not exist. Trying to create", context.Name);
                var createResult = await configurator.CreateDatabaseAsync(configurator.Config);
                if (!createResult)
                {
                    logger.LogWarning("Failed to initialize db {db}", context.Name);
                }
            }
        }
    }
}
