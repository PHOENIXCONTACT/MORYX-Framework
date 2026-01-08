// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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

namespace Moryx.Launcher
{
    /// <inheritdoc />
    public class ShellNavigator : IShellNavigator
    {
        private readonly ILogger _logger;
        private readonly MoryxAccessManagementClient _client;
        private readonly IReadOnlyList<ExternalModuleItem> _externalModules;
        private readonly LauncherConfig _launcherConfig;

        private readonly EndpointDataSource _endpointsDataSource;
        private readonly PageLoader _pageLoader;

        public ShellNavigator(
            EndpointDataSource endpointsDataSource,
            PageLoader pageLoader,
            IConfigManager configManager,
            IOptionsMonitor<MoryxIdentityOptions> options,
            IMemoryCache memoryCache,
            ILoggerFactory logger)
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
            _externalModules = LoadExternalModules();
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ModuleItem>> GetModuleItemsAsync(HttpContext context)
        {
            // Filter pages
            var pageActionDescriptors = _endpointsDataSource.Endpoints.SelectMany(endpoint => endpoint.Metadata)
                .OfType<PageActionDescriptor>()
                .Where(pad => !pad.ViewEnginePath.Contains("Index"));
            // Retrieve all Metadata
            var compiledPageActionDescriptors = await Task.WhenAll(pageActionDescriptors.Select(async pad => await _pageLoader.LoadAsync(pad, EndpointMetadataCollection.Empty)));

            // Filter permission
            if (context is not null && _client is not null)
            {
                var token = context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
                var refreshToken = context.Request.Cookies[MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME];

                var permissions = await _client.GetPermissionsAsync(token, refreshToken);
                compiledPageActionDescriptors = compiledPageActionDescriptors.Where(cpad =>
                {
                    var requiredPolicy = (cpad.EndpointMetadata.SingleOrDefault(a => a is AuthorizeAttribute) as AuthorizeAttribute)?.Policy;
                    return requiredPolicy is null || permissions?.Contains(requiredPolicy) == true;
                }).ToArray();
            }

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
            var webModuleAttribute = pageActionDescriptor.EndpointMetadata.SingleOrDefault(a => a is WebModuleAttribute) as WebModuleAttribute;
            if (webModuleAttribute is null)
                return null;

            var streamAttribute = pageActionDescriptor.EndpointMetadata.SingleOrDefault(a => a is ModuleEventStreamAttribute) as ModuleEventStreamAttribute;
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
}
