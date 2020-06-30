// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;
using Marvin.Model.PostgreSQL;

namespace Marvin.Products.Model
{
    /// <summary>
    /// Factory to get a unit of work for the resources model
    /// </summary>
    public abstract class ProductsUnitOfWorkFactory<TContext> : UnitOfWorkFactoryBase<TContext, NpgsqlModelConfigurator>
        where TContext : ProductsContext
    {
        /// <inheritdoc />
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

    [ModelFactory(ProductsConstants.Namespace)]
    public class ProductUnitOfWorkFactory : ProductsUnitOfWorkFactory<ProductsContext>
    {
    }
}
