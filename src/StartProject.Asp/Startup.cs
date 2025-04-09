using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Moryx.Asp.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace StartProject.Asp
{
    public class Startup
    {
        public IConfiguration Config { get; }

        public Startup(IConfiguration config)
        {
            Config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddRazorPages();

            services.AddControllers(options =>
            {
                options.Filters.Add<MoryxExceptionFilter>();
            })
                .AddJsonOptions(jo => jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(api => ((ControllerActionDescriptor)api.ActionDescriptor).MethodInfo.Name);
            });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder => builder
                .WithOrigins("http://localhost:4200", "http://localhost:8080") // Angular app url for testing purposes
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
            });


            services.AddSwaggerGen(c =>
            {
                c.CustomOperationIds(api => ((ControllerActionDescriptor)api.ActionDescriptor).MethodInfo.Name);
            });

            services.AddSingleton<IAuthorizationPolicyProvider, ExamplePolicyProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            if (env.IsDevelopment())
            {
                app.UseCors("CorsPolicy");
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                var conventionBuilder = endpoints.MapControllers();
                endpoints.MapRazorPages();
                conventionBuilder.WithMetadata(new AllowAnonymousAttribute());
            });
        }
    }

    public class ExamplePolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public ExamplePolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .RequireClaim("Permission", policyName)
                    .Build();               
            }
            return policy;
        }
    }
}
