// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The public API of the ConnectorEntity repository.
    /// </summary>
    public interface IConnectorEntityRepository : IRepository<ConnectorEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ConnectorEntity Create(long connectorId, int classification); 
    }
}
