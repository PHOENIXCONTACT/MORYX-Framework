// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moryx.Container;

namespace Moryx.TestModule;

[Component(LifeCycle.Singleton, typeof(IServiceProviderBridgePlugin))]
public class ServiceProviderBridgePlugin : IServiceProviderBridgePlugin
{
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILogger _logger;

    public ServiceProviderBridgePlugin(IHostEnvironment hostEnvironment, ILogger logger)
    {
        _hostEnvironment = hostEnvironment;
        _logger = logger;
    }

    public string GetEnvironmentName()
    {
        _logger.Log(LogLevel.Information, "Resolved IHostEnvironment via ServiceProvider bridge: {Environment}", _hostEnvironment.EnvironmentName);
        return _hostEnvironment.EnvironmentName;
    }
}
