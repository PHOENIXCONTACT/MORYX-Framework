// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Controllers;
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

builder.Services.AddControllers()
    .AddJsonOptions(jo => jo.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddSwaggerGen(c =>
{
    c.CustomOperationIds(api => ((ControllerActionDescriptor)api.ActionDescriptor).MethodInfo.Name);
});

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
    app.UseSwagger();
    app.UseSwaggerUI();
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
