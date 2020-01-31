// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Reflection;
using Moryx.Container;
using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Factory to create local containers of <see cref="IServerModule"/>
    /// </summary>
    [KernelComponent(typeof(IModuleContainerFactory))]
    public class ModuleContainerFactory : IModuleContainerFactory
    {
        /// <inheritdoc />
        public IContainer Create(IDictionary<Type, string> strategies, Assembly moduleAssembly)
        {
            var container = new LocalContainer(strategies)
                .ExecuteInstaller(new AutoInstaller(moduleAssembly));
            return container;
        }
    }
}
