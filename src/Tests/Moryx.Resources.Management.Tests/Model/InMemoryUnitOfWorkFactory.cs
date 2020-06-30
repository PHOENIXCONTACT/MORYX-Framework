// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.InMemory;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model
{
    /// <summary>
    /// Factory to create in memory databases
    /// </summary>
    public class InMemoryUnitOfWorkFactory : InMemoryUnitOfWorkFactoryBase<ResourcesContext>
    {
        /// <summary>
        /// Creates a new instance of this factory
        /// </summary>
        public InMemoryUnitOfWorkFactory(string instanceId) : base(instanceId)
        {
            
        }

        /// <inheritdoc />
        protected override void Configure()
        {
            RegisterRepository<IResourceEntityRepository>();
            RegisterRepository<IResourceRelationRepository>();
        }
    }
}
