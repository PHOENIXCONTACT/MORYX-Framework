// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model.InMemory;
using Marvin.Products.Model;

namespace Marvin.Products.IntegrationTests
{
    public class InMemoryUnitOfWorkFactory : InMemoryUnitOfWorkFactoryBase<ProductsContext>
    {
        public InMemoryUnitOfWorkFactory(string instanceId) : base(instanceId)
        {
        }

        protected override void Configure()
        {
            RegisterRepository<IProductInstanceEntityRepository>();
            RegisterRepository<IConnectorEntityRepository>();
            RegisterRepository<IConnectorReferenceRepository>();
            RegisterRepository<IOutputDescriptionEntityRepository>();
            RegisterRepository<IPartLinkRepository>();
            RegisterRepository<IProductTypeEntityRepository>();
            RegisterRepository<IProductPropertiesRepository>();
            RegisterRepository<IProductRecipeEntityRepository>();
            RegisterRepository<IStepEntityRepository>();
            RegisterRepository<IWorkplanEntityRepository>();
            RegisterRepository<IWorkplanReferenceRepository>();
        }
    }
}
