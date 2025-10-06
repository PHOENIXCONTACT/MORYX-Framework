// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be used to assign a product to the operation based on existing data.
    /// </summary>
    public interface IPartsAssignment : IConfiguredPlugin<PartsAssignmentConfig>
    {
        /// <summary>
        /// Will be called while creating an operation to load the part list for the new operation
        /// </summary>
        Task<IEnumerable<ProductPart>> LoadParts(Operation operation, IOperationLogger operationLogger);
    }
}