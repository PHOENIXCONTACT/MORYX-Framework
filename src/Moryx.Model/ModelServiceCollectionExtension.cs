// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;

namespace Moryx.Model;

/// <summary>
/// Register components necessary for 
/// </summary>
public static class ModelServiceCollectionExtension
{
    /// <summary>
    /// Link MORYX kernel to the service collection
    /// </summary>
    public static void AddMoryxModels(this IServiceCollection serviceCollection)
    {
        // Register config manager
        serviceCollection.AddSingleton<IDbContextManager, DbContextManager>();
    }
}