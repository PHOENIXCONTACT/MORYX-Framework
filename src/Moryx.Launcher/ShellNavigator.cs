// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moryx.Asp.Integration;
using Moryx.Identity;
using Moryx.Launcher.Config;
using Moryx.Tools;

namespace Moryx.Launcher
{
    /// <inheritdoc />
    public class ShellNavigator : IShellNavigator
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly MoryxAccessManagementClient _client;

        public EndpointDataSource EndpointsDataSource { get; }
        public PageLoader PageLoader { get; }

        public ShellNavigator(
            EndpointDataSource endpointsDataSource,
            PageLoader pageLoader,
            IConfiguration configuration,
            IOptionsMonitor<MoryxIdentityOptions> options,
            IMemoryCache memoryCache,
            ILoggerFactory logger)
        {
            EndpointsDataSource = endpointsDataSource;
            PageLoader = pageLoader;
            _configuration = configuration;
            _logger = logger.CreateLogger(nameof(ShellNavigator));
            if (options?.CurrentValue?.BaseAddress is not null)
            {
                _client = new MoryxAccessManagementClient(
                    options,
                    memoryCache,
                    logger.CreateLogger($"{nameof(ShellNavigator)}:{nameof(MoryxAccessManagementClient)}")
                );
            }
        }

        /// <inheritdoc />
        public async Task<IReadOnlyList<ModuleItem>> GetModuleItems(HttpContext context)
        {
            // Filter pages
            var pageActionDescriptors = EndpointsDataSource.Endpoints.SelectMany(endpoint => endpoint.Metadata)
                .OfType<PageActionDescriptor>()
                .Where(pad => !pad.ViewEnginePath.Contains("Index"));
            // Retrieve all Metadata
            var compiledPageActionDescriptors = await Task.WhenAll(pageActionDescriptors.Select(async pad => await PageLoader.LoadAsync(pad, EndpointMetadataCollection.Empty)));

            // Filter permission
            if (context is not null && _client is not null)
            {
                var token = context.Request.Cookies[MoryxIdentityDefaults.JWT_COOKIE_NAME];
                var refreshToken = context.Request.Cookies[MoryxIdentityDefaults.REFRESH_TOKEN_COOKIE_NAME];

                var permissions = await _client.GetPermissions(token, refreshToken);
                compiledPageActionDescriptors = compiledPageActionDescriptors.Where(cpad =>
                {
                    var requiredPolicy = (cpad.EndpointMetadata.SingleOrDefault(a => a is AuthorizeAttribute) as AuthorizeAttribute)?.Policy;
                    return requiredPolicy is null || permissions?.Contains(requiredPolicy) == true;
                }).ToArray();
            }

            // Load modules
            var modules = compiledPageActionDescriptors.Select(CreateWebModuleItem)
                .Where(m => m != null).ToList<ModuleItem>();

            // Load external modules
            var externalModuleConfigs = _configuration.GetSection("Shell:ExternalModules").Get<ExternalModuleConfig[]>();
            if (externalModuleConfigs != null)
            {
                var externalModules = externalModuleConfigs.Select(CreateExternalModuleItem).ToList();
                modules.AddRange(externalModules);
            }

            // Rudimentary sorting
            var index = 0;
            foreach (var module in modules.OrderBy(m => m.Title))
            {
                var route = module is ExternalModuleItem ? module.Route.Replace("external/", "") : module.Route;
                var sortIndex = _configuration[$"Shell:SortIndex:{route}"];
                module.SortIndex = int.TryParse(sortIndex, out var customIndex) ? customIndex : index++;
            }

            return modules;
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
                EventStream = streamAttribute?.EventStreamUrl ?? null
            };
        }
    }
}
