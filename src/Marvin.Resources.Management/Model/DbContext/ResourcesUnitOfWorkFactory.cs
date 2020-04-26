// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;
using Marvin.Model.PostgreSQL;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
{
    /// <summary>
    /// Factory to get a unit of work for the resources model
    /// </summary>
    [ModelFactory(ResourcesConstants.Namespace)]
    public class ResourcesUnitOfWorkFactory : UnitOfWorkFactoryBase<ResourcesContext, NpgsqlModelConfigurator>
    {
        /// <inheritdoc />
        protected override void Configure()
        {
            RegisterRepository<IResourceEntityRepository>();
            RegisterRepository<IResourceRelationRepository>();
        }
    }
}
