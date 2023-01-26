// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Container;

namespace Moryx.TestTools.UnitTest
{
    /// <summary>
    /// Installer registering the interceptor
    /// </summary>
    public class BasicInterceptorInstaller : IContainerInstaller
    {
        /// <inheritdoc />
        public void Install(IComponentRegistrator registrator)
        {
            registrator.Register(typeof(EmptyInterceptor));
            registrator.Register(typeof(NullLoggerFactory), new []{typeof(ILoggerFactory)});
        }
    }
}
