// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Moryx.AspNetCore;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Simple helper to configure the two separate json options in aspnet core at the same time.
        /// Helpful when using Controllers and MinimalApis to be consistent or when using Microsoft.AspNetCore.OpenApi,
        /// because it looks at the mimimal api options even for controllers
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public IServiceCollection ConfigureJsonOptions(Action<JsonSerializerOptions> configure)
        {
            services.ConfigureHttpJsonOptions(options => configure(options.SerializerOptions));
            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options => configure(options.JsonSerializerOptions));

            return services;
        }
    }
}
