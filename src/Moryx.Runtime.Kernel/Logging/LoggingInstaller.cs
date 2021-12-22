// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Moryx.Container;

namespace Moryx.Runtime.Kernel
{
    internal class LoggingInstaller : IContainerInstaller
    {
        public void Install(IComponentRegistrator registrator)
        {
            registrator.Register(typeof(LoggerFactory), new[] { typeof(ILoggerFactory) }, "LoggerFactory", LifeCycle.Singleton);
            registrator.Register(typeof(Logger<>), new[] { typeof(ILogger<>) }, "Logger", LifeCycle.Singleton);
        }
    }
}
