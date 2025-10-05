// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        public async Task<IReadOnlyList<WebModuleItem>> GetWebModuleItems(HttpContext context)
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

            var webModules = compiledPageActionDescriptors.Select(cpad => CreateWebModuleItem(cpad))
                .OfType<WebModuleItem>().ToList();

            var index = 0;
            // Rudimentary sorting
            foreach (var webModule in webModules.OrderBy(wmi => wmi.Route))
            {
                // See if custom index was configured for the module
                var sortIndex = _configuration[$"Shell:SortIndex:{webModule.Route}"];
                if (int.TryParse(sortIndex, out var customIndex))
                    webModule.SortIndex = customIndex;
                else
                    webModule.SortIndex = index++;
            }

            return webModules;
        }

        private WebModuleItem CreateWebModuleItem(CompiledPageActionDescriptor pageActionDescriptor)
        {
            var webModuleAttribute = pageActionDescriptor.EndpointMetadata.SingleOrDefault(a => a is WebModuleAttribute) as WebModuleAttribute;
            if (webModuleAttribute is null)
                return null;

            var streamAttribute = pageActionDescriptor.EndpointMetadata.SingleOrDefault(a => a is ModuleEventStreamAttribute) as ModuleEventStreamAttribute;
            return new WebModuleItem()
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

