// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moryx.Identity.AccessManagement.Data;
using Moryx.Identity.AccessManagement.Settings;

namespace Moryx.Identity.AccessManagement;

/// <summary>
/// Provid extension method for an easy configuration of the MORYX Access Management in the <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds and configures services for the MORYX AccessManagement to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="jwtConfigurationSection">The section of the configuration containing the <see cref="JwtSettings"/>.</param>
    /// <param name="connectionString">The connection string for the PostgreSql database used by the MORYX AccessManagement.</param>
    /// <param name="corsOptionsAction">Action providing CORS options used for a call to
    /// <see cref="CorsServiceCollectionExtensions.AddCors(IServiceCollection, Action{CorsOptions})"/>.</param>
    /// <remarks>
    /// This method configures the <see cref="IServiceCollection"/> to use the MORYX AccessManagement with a PostgreSql
    /// database provider.
    /// It combines MORYX identity specific service registrations with the effects of
    /// <see cref="EntityFrameworkServiceCollectionExtensions.AddDbContext{MoryxIdentitiesDbContext}(IServiceCollection, Action{DbContextOptionsBuilder}?, ServiceLifetime, ServiceLifetime)"/>
    /// <see cref="IdentityServiceCollectionExtensions.AddIdentity{MoryxUser, MoryxRole}(IServiceCollection, Action{IdentityOptions})"/>
    /// using the <see cref="MoryxUserManager"/>, the <see cref="MoryxRoleManager"/> and the <see cref="MoryxIdentitiesDbContext"/>,
    /// <see cref="PolicyServiceCollectionExtensions.AddAuthorization(IServiceCollection)"/>
    /// <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication(IServiceCollection, Action{Microsoft.AspNetCore.Authentication.AuthenticationOptions})"/>
    /// using the <see cref="JwtBearerExtensions.AddJwtBearer(Microsoft.AspNetCore.Authentication.AuthenticationBuilder, Action{JwtBearerOptions})"/>
    /// extension to configure JWT authentication,
    /// <see cref="CorsServiceCollectionExtensions.AddCors(IServiceCollection, Action{CorsOptions})"/> using the provided <paramref name="corsOptionsAction"/>
    /// open up the MORYX AccesManagement to other applications,
    /// <see cref="MvcServiceCollectionExtensions.AddControllersWithViews(IServiceCollection)"/>.
    /// </remarks>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMoryxAccessManagement(this IServiceCollection services,
        IConfiguration jwtConfigurationSection,
        string connectionString,
        Action<CorsOptions> corsOptionsAction = null)
    {
        return services.AddMoryxAccessManagement(jwtConfigurationSection, options =>
            options.UseNpgsql(connectionString), corsOptionsAction);
    }

    /// <summary>
    /// Adds and configures services for the MORYX AccessManagement to the specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="jwtConfigurationSection">The section of the configuration containing the <see cref="JwtSettings"/>.</param>
    /// <param name="dbOptionsAction">A <see cref="Action{DbOptionsContextBuilder}"/> tp use a custom database provider.</param>
    /// <param name="corsOptionsAction">Action providing CORS options used for a call to
    /// <see cref="CorsServiceCollectionExtensions.AddCors(IServiceCollection, Action{CorsOptions})"/>.</param>
    /// <remarks>
    /// This method configures the <see cref="IServiceCollection"/> to use the MORYX AccessManagement with a custom
    /// database provider.
    /// It combines MORYX identity specific service registrations with the effects of
    /// <see cref="EntityFrameworkServiceCollectionExtensions.AddDbContext{MoryxIdentitiesDbContext}(IServiceCollection, Action{DbContextOptionsBuilder}?, ServiceLifetime, ServiceLifetime)"/>
    /// <see cref="IdentityServiceCollectionExtensions.AddIdentity{MoryxUser, MoryxRole}(IServiceCollection, Action{IdentityOptions}?)"/>
    /// using the <see cref="MoryxUserManager"/>, the <see cref="MoryxRoleManager"/> and the <see cref="MoryxIdentitiesDbContext"/>,
    /// <see cref="PolicyServiceCollectionExtensions.AddAuthorization(IServiceCollection)"/>
    /// <see cref="AuthenticationServiceCollectionExtensions.AddAuthentication(IServiceCollection, Action{Microsoft.AspNetCore.Authentication.AuthenticationOptions})"/>
    /// using the <see cref="JwtBearerExtensions.AddJwtBearer(Microsoft.AspNetCore.Authentication.AuthenticationBuilder, Action{JwtBearerOptions})"/>
    /// extension to configure JWT authentication,
    /// <see cref="CorsServiceCollectionExtensions.AddCors(IServiceCollection, Action{CorsOptions})"/> using the provided <paramref name="corsOptionsAction"/>
    /// open up the MORYX AccesManagement to other applications,
    /// <see cref="MvcServiceCollectionExtensions.AddControllersWithViews(IServiceCollection)"/>.
    /// </remarks>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection AddMoryxAccessManagement(this IServiceCollection services,
        IConfiguration jwtConfigurationSection,
        Action<DbContextOptionsBuilder> dbOptionsAction,
        Action<CorsOptions> corsOptionsAction = null)
    {

        services.Configure<JwtSettings>(jwtConfigurationSection);
        // Explicitly register the settings object by delegating to the IOptions object
        services.AddSingleton(resolver =>
            resolver.GetRequiredService<IOptions<JwtSettings>>().Value);

        // Register Identity
        services.AddDbContext<MoryxIdentitiesDbContext>(dbOptionsAction);

        services.AddIdentity<MoryxUser, MoryxRole>(options =>
            {
                options.Stores.MaxLengthForKeys = 128;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddUserManager<MoryxUserManager>()
            .AddRoleManager<MoryxRoleManager>()
            .AddEntityFrameworkStores<MoryxIdentitiesDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUserClaimsPrincipalFactory<MoryxUser>, MoryxClaimsPrincipalFactory>();
        services.AddScoped<IPermissionManager, PermissionManager>();

        // TokenService
        var jwtSettings = jwtConfigurationSection.Get<JwtSettings>();
        var tokenValidationParams = new TokenValidationParameters // Extract
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            //ClockSkew = TimeSpan.Zero,
            AuthenticationType = IdentityConstants.ApplicationScheme
        };
        services.AddSingleton(tokenValidationParams);
        services.AddTransient<ITokenService, TokenService>();
        services.AddTransient<IPasswordResetService, PasswordResetService>();

        // Add Authorization & Authentication
        // services.AddAuthorization();

        services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = tokenValidationParams;
                // Copy HttpOnly cookie to Beare Header
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddCors(corsOptionsAction);

        // Add MVC
        services.AddControllersWithViews()
            .AddJsonOptions(jo => jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return services;
    }
}