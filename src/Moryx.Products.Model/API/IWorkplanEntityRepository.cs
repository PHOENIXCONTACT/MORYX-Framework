// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using Moryx.Model.Repositories;

namespace Moryx.Products.Model
{
    /// <summary>
    /// The public API of the WorkplanEntity repository.
    /// </summary>
    public interface IWorkplanEntityRepository : IRepository<WorkplanEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        WorkplanEntity Create(string name, int version, int state); 
    }
}
