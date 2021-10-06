// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Default StartUp used by MORYX Kestrel Hosting
    /// </summary>
    public class Startup
    {
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddJsonOptions(options => {
                    //options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.IgnoreNullValues = false;
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                })
                .ConfigureApplicationPartManager(manager =>
                {
                    // Find all assemblies with defined controllers and add them to the application parts
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies.Where(a =>
                        manager.ApplicationParts.All(p => p.Name != a.GetName().Name)))
                    {
                        if (assembly.DefinedTypes.Any(t => typeof(Controller).IsAssignableFrom(t)))
                        {
                            manager.ApplicationParts.Add(new AssemblyPart(assembly));
                        }
                    }

                    manager.FeatureProviders.Add(new CustomControllerFeatureProvider());
                })
                .AddControllersAsServices();

            services.AddAuthorization();
            services.AddCors();
        }

        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
