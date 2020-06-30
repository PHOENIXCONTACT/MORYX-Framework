// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the ConnectorReference repository.
    /// </summary>
    public interface IConnectorReferenceRepository : IRepository<ConnectorReference>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ConnectorReference Create(int index, ConnectorRole role); 
    }
}
