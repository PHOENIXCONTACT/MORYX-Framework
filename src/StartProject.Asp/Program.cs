// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Moryx.AspNetCore.OpenApi;
using Moryx.Launcher;
using Moryx.Model;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Modules;
using Moryx.Tools;
using StartProject.Asp;

AppDomainBuilder.LoadAssemblies();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMoryxKernel();
builder.Services.AddMoryxModels();
builder.Services.AddMoryxModules();
builder.Services.AddMoryxLauncher();

builder.Services.AddLocalization();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("de-DE"),
        new CultureInfo("en-US"),
        new CultureInfo("it-IT"),
        new CultureInfo("zh-Hans"),
        new CultureInfo("pl-PL"),
    };

    options.DefaultRequestCulture = new RequestCulture(culture: "de-DE", uiCulture: "de-DE");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy => policy
        .WithOrigins("http://localhost:4200") // Angular app url for testing purposes
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());
});

builder.Services.AddRazorPages();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.NumberHandling = JsonNumberHandling.Strict; // https://github.com/dotnet/aspnetcore/issues/64501
});
builder.Services.AddControllers();

builder.Services.AddOpenApi(static options =>
{

    options.OpenApiVersion = OpenApiSpecVersion.OpenApi3_1; // OpenApi3_2 is only available with .net 11
    //options.AddOperationTransformer<SseOpenApiMapper>();
    options.ConfigureMoryxTransformers();
}
);

builder.Services.AddSingleton<IAuthorizationPolicyProvider, ExamplePolicyProvider>();

var app = builder.Build();

app.Services.UseMoryxConfigurations("Config");

var pathBase = app.Configuration.GetValue<string>("PathBase");
if (!string.IsNullOrEmpty(pathBase))
{
    // Enable prefix striping. The path base is removed from the path and witten to 
    // Context.Request.PathBase for later usage
    app.UsePathBase(pathBase);

    // Middleware to specifially block all traffic, that does not have the path base set for testing.
    // This is not necessary for normal applications and only used for manual tests to ensure the information about
    // the path base has been propagated through to all the razor pages and javascript applications that request
    // data from the server.
    app.Use(new PathBaseTestMiddleware(pathBase).BlockRequestsWithoutPathBase);
}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.MapOpenApi();
    // Map open api spec as yaml to, if someone requires it
    app.MapGet("/openapi/v1.yaml",
        async ([FromKeyedServices("v1")] IOpenApiDocumentProvider provider) =>
        {
            var document = await provider.GetOpenApiDocumentAsync();

            using var stringWriter = new StringWriter();
            var writer = new OpenApiYamlWriter(stringWriter);

            document.SerializeAsV31(writer);

            return Results.Text(
                stringWriter.ToString(),
            "application/yaml");
        });
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/openapi/v1.json", "v1")
    );
}
app.UseRequestLocalization();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseCors("CorsPolicy");
}

app.UseAuthorization();

app.MapControllers().WithMetadata(new AllowAnonymousAttribute());
app.MapRazorPages();

var moduleManager = app.Services.GetRequiredService<IModuleManager>();
await moduleManager.StartModulesAsync();

await app.RunAsync();

await moduleManager.StopModulesAsync();
