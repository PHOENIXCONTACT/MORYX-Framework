// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.Products.Samples.Model
{
    /// <summary>
    /// The public API of the ProductProperties repository.
    /// </summary>
    public interface ISmartWatchProductPropertiesEntityRepository : IRepository<SmartWatchProductPropertiesEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        SmartWatchProductPropertiesEntity Create(int state); 
    }
}
