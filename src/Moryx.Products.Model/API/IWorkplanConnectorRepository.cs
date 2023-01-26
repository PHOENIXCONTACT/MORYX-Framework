// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model.Repositories;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The public API of the ConnectorEntity repository.
    /// </summary>
    public interface IWorkplanConnectorRepository : IRepository<WorkplanConnectorEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        WorkplanConnectorEntity Create(long connectorId, int classification); 
    }
}
