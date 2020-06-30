// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The public API of the PartLink repository.
    /// </summary>
    public interface IPartLinkRepository : IRepository<PartLink>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        PartLink Create(string propertyName); 
    }
}
