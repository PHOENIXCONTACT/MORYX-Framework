// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

// ReSharper disable once CheckNamespace
namespace Marvin.Resources.Model
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
