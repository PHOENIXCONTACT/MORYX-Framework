// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moryx.Model.PostgreSQL;

namespace Moryx.Identity.AccessManagement.Data;

/// <summary>
/// Design time factory for the <see cref="MoryxIdentitiesDbContext"/>
/// </summary>
public class MoryxIdentitiesDbContextDesignTimeFactory : NpgsqlDesignTimeDbContextFactory<MoryxIdentitiesDbContext>
{
    /// <inheritdoc/>
    public override MoryxIdentitiesDbContext CreateDbContext(string[] args)
    {
        var connectionString = ResolveConnectionString(args);

        var services = new ServiceCollection();
        AddIdentity(services);
        var serviceProvider = services.BuildServiceProvider();
        var optionsBuilder = new DbContextOptionsBuilder<MoryxIdentitiesDbContext>();
        optionsBuilder.UseApplicationServiceProvider(serviceProvider);
        optionsBuilder.UseNpgsql(connectionString);
        return new MoryxIdentitiesDbContext(optionsBuilder.Options);
    }

    private static IServiceCollection AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<MoryxUser, MoryxRole>(options =>
        {
            options.Stores.MaxLengthForKeys = 128;
            options.SignIn.RequireConfirmedAccount = false;
        })
                .AddUserManager<MoryxUserManager>()
                .AddRoleManager<MoryxRoleManager>()
                .AddEntityFrameworkStores<MoryxIdentitiesDbContext>()
                .AddDefaultTokenProviders();

        return services;
    }
}
