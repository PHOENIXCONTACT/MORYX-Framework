// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moryx.Configuration;
using Moryx.Identity;
using Moryx.Modules;
using Moryx.Tools;

namespace Moryx.Launcher;

/// <summary>
/// Component to determine navigation items for the shell
/// </summary>
internal class Navigation : INavigation
{
    private readonly MoryxAccessManagementClient _client;
    private readonly IReadOnlyList<ExternalModuleItem> _externalModules;
    private readonly LauncherConfig _launcherConfig;

    private readonly EndpointDataSource _endpointsDataSource;
    private readonly PageLoader _pageLoader;

    public Navigation(
        EndpointDataSource endpointsDataSource,
        PageLoader pageLoader,
        IConfigManager configManager,
        IOptionsMonitor<MoryxIdentityOptions> options,
        IMemoryCache memoryCache,
        ILoggerFactory logger)
    {
        _endpointsDataSource = endpointsDataSource;
        _pageLoader = pageLoader;
        if (options?.CurrentValue?.BaseAddress is not null)
        {
            _client = new MoryxAccessManagementClient(
                options,
                memoryCache,
                logger.CreateLogger($"{nameof(Navigation)}:{nameof(MoryxAccessManagementClient)}")
            );
        }

        _launcherConfig = GetConfiguration(configManager);
        _externalModules = LoadExternalModules();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<ModuleItem>> GetModuleItemsAsync(HttpContext context)
    {
        var compiledPageActionDescriptors = await CompiledPageActionDescriptors(context);

        // Load modules
        var modules = compiledPageActionDescriptors.Select(CreateWebModuleItem)
            .Where(m => m != null).ToList<ModuleItem>();

        modules.AddRange(_externalModules);

        // Rudimentary sorting
        var index = 0;
        foreach (var module in modules.OrderBy(m => m.Title))
        {
            var route = module is ExternalModuleItem ? module.Route.Replace("external/", "") : module.Route;
            var indexConfig = _launcherConfig.ModuleSortIndices.FirstOrDefault(m => m.Route == route);
            if (indexConfig != null)
            {
                module.SortIndex = indexConfig.SortIndex;
                continue;
            }

            module.SortIndex = index++;
        }

        return modules;
    }

    private async Task<CompiledPageActionDescriptor[]> CompiledPageActionDescriptors(HttpContext context)
    {
        // Filter pages
        var pageActionDescriptors = _endpointsDataSource.Endpoints.SelectMany(endpoint => endpoint.Metadata)
            .OfType<PageActionDescriptor>()
            .Where(pad => !pad.ViewEnginePath.Contains("Index"));
        // Retrieve all Metadata
        var compiledPageActionDescriptors = await Task.WhenAll(pageActionDescriptors.Select(async pad =>
            await _pageLoader.LoadAsync(pad, EndpointMetadataCollection.Empty)));

        // Filter permission
        if (context is not null && _client is not null)
        {
            var token = context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
            var refreshToken = context.Request.Cookies[MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME];

            var permissions = await _client.GetPermissionsAsync(token, refreshToken);
            compiledPageActionDescriptors = compiledPageActionDescriptors.Where(cpad =>
            {
                var requiredPolicy =
                    (cpad.EndpointMetadata.SingleOrDefault(a => a is AuthorizeAttribute) as AuthorizeAttribute)?.Policy;
                return requiredPolicy is null || permissions?.Contains(requiredPolicy) == true;
            }).ToArray();
        }

        return compiledPageActionDescriptors;
    }

    public RegionItem GetRegion(LauncherRegion region)
    {
        var availableAssemblies = ReflectionTool.GetAssemblies();

        // Retrieve views
        var partialViews = new List<Type>(availableAssemblies.Length * 30);
        foreach (var assembly in availableAssemblies)
        {
            try
            {
                var types = assembly.GetTypes().Where(t => t.IsClass && t.GetCustomAttribute<LauncherRegionAttribute>() != null);
                partialViews.AddRange(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check for partial views classes in assembly {name}.", assembly.FullName);
            }
        }

        // Transform to models
        var configuredRegions = from pV in partialViews
                                let regionAttr = pV.GetCustomAttribute<LauncherRegionAttribute>()
                                let config = _launcherConfig.Regions.FirstOrDefault(x => x.Name == regionAttr.Name)
                                where config != null
                                select new RegionItem { PartialView = regionAttr.Name, Region = config.Region };

        return configuredRegions.FirstOrDefault(r => r.Region == region);
    }

    private ExternalModuleItem[] LoadExternalModules()
    {
        var externalModuleConfigs = _launcherConfig.ExternalModules;
        return externalModuleConfigs?.Select(CreateExternalModuleItem).ToArray() ?? [];
    }

    private static ExternalModuleItem CreateExternalModuleItem(ExternalModuleConfig externalModuleConfig)
    {
        return new ExternalModuleItem
        {
            Title = externalModuleConfig.Title,
            Description = externalModuleConfig.Description,
            Url = externalModuleConfig.Url,
            Icon = externalModuleConfig.Icon,
            Category = ModuleCategory.User,
            Route = $"external/{externalModuleConfig.Route}"
        };
    }
    private static WebModuleItem CreateWebModuleItem(CompiledPageActionDescriptor pageActionDescriptor)
    {
        var webModuleAttribute =
            pageActionDescriptor.EndpointMetadata.SingleOrDefault(a => a is WebModuleAttribute) as WebModuleAttribute;
        if (webModuleAttribute is null)
            return null;

        var streamAttribute =
            pageActionDescriptor.EndpointMetadata.SingleOrDefault(a => a is ModuleEventStreamAttribute) as
                ModuleEventStreamAttribute;
        return new WebModuleItem
        {
            Title = pageActionDescriptor.PageTypeInfo.GetDisplayName() ?? webModuleAttribute.Route,
            Route = webModuleAttribute.Route,
            Icon = webModuleAttribute.Icon,
            Description = pageActionDescriptor.PageTypeInfo.GetDescription() ?? "",
            Category = webModuleAttribute.Category,
            EventStream = streamAttribute?.EventStreamUrl
        };
    }

    private LauncherConfig GetConfiguration(IConfigManager configManager)
    {
        var launcherConfig = configManager.GetConfiguration<LauncherConfig>();

        // If configuration is generated, save it back to persist defaults
        if (launcherConfig.ConfigState == ConfigState.Generated)
        {
            launcherConfig.ConfigState = ConfigState.Valid;
            configManager.SaveConfiguration(launcherConfig);
        }

        return launcherConfig;
    }
}
