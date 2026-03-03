// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.DependencyInjection;

namespace Moryx.Launcher;

/// <summary>
/// Extension methods for setting up MORYX Launcher services in an <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds services required for application localization.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to add the services to.</param>
    /// <returns>>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMoryxLauncher(this IServiceCollection services)
    {
        services.AddSingleton<IShellNavigator, ShellNavigator>();

        return services;
    }
}
