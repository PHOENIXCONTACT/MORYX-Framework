// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Concurrent;
using System.Globalization;
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
//TODO: make it internal in next major
/// <inheritdoc />
public class ShellNavigator : IShellNavigator, ILauncher
{
    private const string NotificationsBarName = "NotificationsBar";

    private static ILogger _logger;
    private static PageLoader _pageLoader;
    private static EndpointDataSource _endpointsDataSource;

    private readonly MoryxAccessManagementClient _client;
    private static LauncherConfig _launcherConfig;

    private static readonly ConcurrentDictionary<string, PageActionDescriptorAndModuleItem[]> _descriptorsAndModules = new();
    private static readonly Lazy<RegionItem[]> _configuredRegions = new(LoadRegions, LazyThreadSafetyMode.ExecutionAndPublication);

    public ShellNavigator(EndpointDataSource endpointsDataSource, PageLoader pageLoader, IConfigManager configManager,
        IOptionsMonitor<MoryxIdentityOptions> options, IMemoryCache memoryCache, ILoggerFactory logger)
    {
        _endpointsDataSource = endpointsDataSource;
        _pageLoader = pageLoader;
        _logger = logger.CreateLogger(nameof(ShellNavigator));
        if (options?.CurrentValue?.BaseAddress is not null)
        {
            _client = new MoryxAccessManagementClient(
                options,
                memoryCache,
                logger.CreateLogger($"{nameof(ShellNavigator)}:{nameof(MoryxAccessManagementClient)}")
            );
        }

        _launcherConfig = GetConfiguration(configManager);
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<ModuleItem>> GetModuleItemsAsync(HttpContext context) =>
        FilterModuleItems(context);

    /// <summary>
    /// Filter <see cref="_descriptorsAndModules"/> by user permissions
    /// </summary>
    /// <param name="context">HttpContext used to extract the users identity tokens</param>
    /// <returns>A filtered array of <see cref="ModuleItem"/>s the user has permission to see</returns>
    private async Task<IReadOnlyList<ModuleItem>> FilterModuleItems(HttpContext context)
    {
        var cultureName = CultureInfo.CurrentUICulture.Name;
        var descriptorsAndModules = _descriptorsAndModules.GetOrAdd(cultureName, _ => LoadCompiledActionDescriptors());

        // No authorization is configured
        if (context is null || _client is null)
        {
            return [.. descriptorsAndModules.Select(t => t.ModuleItem)];
        }

        var token = context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
        var refreshToken = context.Request.Cookies[MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME];

        var permissions = await _client.GetPermissionsAsync(token, refreshToken);
        return [.. descriptorsAndModules.Where(t =>
        {
            var requiredPolicy = t.CompiledPageActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>()
                .SingleOrDefault()?.Policy;
            return requiredPolicy is null || permissions?.Contains(requiredPolicy) == true;
        }).Select(t => t.ModuleItem)];
    }

    RegionItem ILauncher.GetRegion(LauncherRegion region) =>
        _configuredRegions.Value.FirstOrDefault(r => r.Region == region);

    /// <summary>
    /// Load configured launcher regions
    /// </summary>
    private static RegionItem[] LoadRegions()
    {
        var availableAssemblies = ReflectionTool.GetAssemblies();

        // Retrieve views
        var partialViews = new List<Type>(availableAssemblies.Length * 30);
        foreach (var assembly in availableAssemblies)
        {
            try
            {
                var types = assembly.GetTypes().Where(t => t.IsClass && t.IsDefined(typeof(LauncherRegionAttribute), false));
                partialViews.AddRange(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check for partial views classes in assembly {name}.", assembly.FullName);
            }
        }

        // Transform to models
        var regions = (from pV in partialViews
            let regionAttr = pV.GetCustomAttribute<LauncherRegionAttribute>()
            let config = _launcherConfig.Regions.FirstOrDefault(x => x.Name == regionAttr.Name)
            where config != null
            select new RegionItem { PartialView = regionAttr.Name, Region = config.Region }).ToArray();

        return regions;
    }

    /// <summary>
    /// Load the full set of <see cref="CompiledPageActionDescriptor"/>s to be filtered by permissions later on
    /// </summary>
    private static PageActionDescriptorAndModuleItem[] LoadCompiledActionDescriptors()
    {
        // Filter for pages hosted by the application
        var pageActionDescriptors = _endpointsDataSource.Endpoints.SelectMany(endpoint => endpoint.Metadata)
            .OfType<PageActionDescriptor>()
            .Where(pad => !pad.ViewEnginePath.Contains("Index"));

        // Retrieve metaata for pages
        var descriptors = Task.WhenAll(pageActionDescriptors.Select(async pad =>
            await _pageLoader.LoadAsync(pad, EndpointMetadataCollection.Empty))).GetAwaiter().GetResult();

        // Construct mappings between metadata and module items
        var descriptorModuleTuples = descriptors.Select(d => new PageActionDescriptorAndModuleItem(d, CreateWebModuleItem(d)))
            .Where((dam) => dam.ModuleItem != null).ToList();

        // Append external modules without metadata
        var externalModuleTuples = _launcherConfig.ExternalModules?
            .Select(c => new PageActionDescriptorAndModuleItem(null, CreateExternalModuleItem(c))) ?? [];
        descriptorModuleTuples.AddRange(externalModuleTuples);

        // Sort by module item sort indices (and title if sort index is not set)
        var index = _launcherConfig.ModuleSortIndices.Select(i => i.SortIndex).DefaultIfEmpty(0).Max();
        foreach (var descriptorAndModule in descriptorModuleTuples.OrderBy(t => t.ModuleItem.Title))
        {
            var module  = descriptorAndModule.ModuleItem;
            var route = module is ExternalModuleItem ? module.Route.Replace("external/", "") : module.Route;
            var indexConfig = _launcherConfig.ModuleSortIndices.FirstOrDefault(m => m.Route == route);
            if (indexConfig != null)
            {
                module.SortIndex = indexConfig.SortIndex;
                continue;
            }

            module.SortIndex = ++index;
        }

        return [.. descriptorModuleTuples.OrderBy(t => t.ModuleItem.SortIndex)];
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
        var webModuleAttribute = pageActionDescriptor.EndpointMetadata.OfType<WebModuleAttribute>().SingleOrDefault();
        if (webModuleAttribute is null)
            return null;

        var streamAttribute = pageActionDescriptor.EndpointMetadata.OfType<ModuleEventStreamAttribute>().SingleOrDefault();
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

    private static LauncherConfig GetConfiguration(IConfigManager configManager)
    {
        var launcherConfig = configManager.GetConfiguration<LauncherConfig>();

        // If configuration is generated, save it back to persist defaults
        if (launcherConfig.ConfigState == ConfigState.Generated)
        {
            launcherConfig.ConfigState = ConfigState.Valid;

            configManager.SaveConfiguration(launcherConfig);
        }

        AddNotificationsBarRegion(configManager, launcherConfig);

        return launcherConfig;
    }

    private static void AddNotificationsBarRegion(IConfigManager configManager, LauncherConfig launcherConfig)
    {
        var topRegion = launcherConfig.Regions.FirstOrDefault(r => r.Region == LauncherRegion.Top);

        if (topRegion == null)
        {
            topRegion = new LauncherRegionConfig
            {
                Region = LauncherRegion.Top,
                Name = NotificationsBarName
            };

            launcherConfig.Regions = [.. launcherConfig.Regions, topRegion];

            configManager.SaveConfiguration(launcherConfig);
        }
    }

    private record struct PageActionDescriptorAndModuleItem(CompiledPageActionDescriptor CompiledPageActionDescriptor, ModuleItem ModuleItem) { }
}
