// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Modules
{
    /// <summary>
    /// Class representing a missing module dependency
    /// </summary>
    internal class MissingModuleDependency : IModuleDependency
    {
        public IServerModule RepresentedModule { get; private set; }

        public MissingModuleDependency(IServerModule representedModule)
        {
            RepresentedModule = representedModule;
        }

        public IReadOnlyList<IModuleDependency> Dependencies => throw new NotImplementedException();

        public IReadOnlyList<IModuleDependency> Dependents => throw new NotImplementedException();
    }
}

