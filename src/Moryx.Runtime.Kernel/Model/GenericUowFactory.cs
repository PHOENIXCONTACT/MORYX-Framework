// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;
using Moryx.Model;

namespace Moryx.Runtime.Kernel
{
    /// <summary>
    /// Generic factory to provide access to all data models
    /// </summary>
    [KernelComponent(typeof(IModelResolver))]
    public class GenericUowFactory : IModelResolver
    {
        /// <summary>
        /// Global container which hold all data models. Injected by castle.
        /// </summary>
        public IContainer GlobalContainer { get; set; }

        /// <summary>
        /// Create an open context using the model namespace
        /// </summary>
        public IUnitOfWorkFactory GetByName(string modelNamespace)
        {
            return GlobalContainer.Resolve<IUnitOfWorkFactory>(modelNamespace);
        }
    }
}
