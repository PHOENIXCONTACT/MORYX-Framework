// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.Repositories;

// ReSharper disable once CheckNamespace
namespace Moryx.Resources.Model
{
    /// <summary>
    /// The public API of the ResourceRelation repository.
    /// </summary>
    public interface IResourceRelationRepository : IRepository<ResourceRelation>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ResourceRelation Create(int relationType);
    }
}
