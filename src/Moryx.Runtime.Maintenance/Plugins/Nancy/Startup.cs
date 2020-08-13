using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Moryx.Container;
using Nancy.Owin;

namespace Moryx.Runtime.Maintenance.Plugins
{
    public class Startup : IStartup
    {
        private readonly IContainer _container;
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env, IContainer container)
        {
            _container = container;
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // Add framework services
            services.AddCors();

            return services.BuildServiceProvider();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UsePathBase("/maintenance");
            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials()
            );

            var filepath = Path.Combine(Directory.GetCurrentDirectory(), "maintenance");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(filepath),
                RequestPath = ""
            });

            app.UseOwin(x => x.UseNancy(opt => opt.Bootstrapper = new MoryxNancyBootstrapper(_container)));
        }
    }
}