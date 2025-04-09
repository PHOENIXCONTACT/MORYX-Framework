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
    public static class BasicInterceptorInstaller
    {
        /// <inheritdoc />
        public static void Install(this IContainer container)
        {
            container.Register(typeof(EmptyInterceptor));
            container.Register(typeof(NullLoggerFactory), [typeof(ILoggerFactory)]);
        }
    }
}
