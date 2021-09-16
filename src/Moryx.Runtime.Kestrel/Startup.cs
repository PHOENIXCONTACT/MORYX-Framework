using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moryx.Configuration;

namespace Moryx.Runtime.Kestrel
{
    /// <summary>
    /// Default StartUp used by MORYX Kestrel Hosting
    /// </summary>
    internal class Startup
    {
        public void ConfigureServices(IServiceCollection services)
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
                    // Find all Controllers and add them to the local container
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    foreach (var assembly in assemblies.Where(a =>
                        manager.ApplicationParts.All(p => p.Name != a.GetName().Name)))
                    {
                        if (assembly.DefinedTypes.Any(t => typeof(Controller).IsAssignableFrom(t)))
                        {
                            manager.ApplicationParts.Add(new AssemblyPart(assembly));
                        }
                    }
                })
                .AddControllersAsServices();

            //services.AddSingleton<IEndpointCollector, EndpointCollector>();

            services.AddAuthorization();

            //services.AddApiVersioning(options =>
            //{
            //    options.DefaultApiVersion = new ApiVersion(1, 0);
            //    options.AssumeDefaultVersionWhenUnspecified = true;
            //    options.ReportApiVersions = true;
            //});
            //services.AddVersionedApiExplorer(
            //    options =>
            //    {
            //        options.GroupNameFormat = "'v'VVV";
            //        options.SubstituteApiVersionInUrl = true;
            //    });

            services.AddCors();

            //services.AddSwaggerGen(options =>
            //{
            //    options.OperationFilter<SwaggerDefaultValues>();

            //    var baseDirectory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            //    foreach (var fi in baseDirectory.EnumerateFiles("*.xml"))
            //    {
            //        options.IncludeXmlComments(fi.FullName);
            //    }
            //});
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            //app.UseSwagger(options =>
            //{
            //    options.RouteTemplate = "/swagger/{documentName}/swagger.json";
            //});
            //app.UseSwaggerUI(options =>
            //{
            //    foreach (var description in provider.ApiVersionDescriptions)
            //    {
            //        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            //            $"MORYX API {description.GroupName.ToUpperInvariant()}");
            //    }

            //    options.RoutePrefix = "swagger";
            //});

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
