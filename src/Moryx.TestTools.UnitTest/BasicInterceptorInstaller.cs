// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Logging;

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
            registrator.Register(typeof(DummyLogger), new []{typeof(IModuleLogger)});
        }
    }
}
