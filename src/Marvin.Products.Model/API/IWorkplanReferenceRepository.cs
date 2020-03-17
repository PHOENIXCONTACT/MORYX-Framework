// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the WorkplanReference repository.
    /// </summary>
    public interface IWorkplanReferenceRepository : IRepository<WorkplanReference>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        WorkplanReference Create(int referenceType); 
    }
}
