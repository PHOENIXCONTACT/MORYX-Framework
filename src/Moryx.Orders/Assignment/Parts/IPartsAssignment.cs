// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be used to assign a product to the operation based on existing data.
    /// </summary>
    public interface IPartsAssignment : IAsyncConfiguredPlugin<PartsAssignmentConfig>
    {
        /// <summary>
        /// Will be called while creating an operation to load the part list for the new operation
        /// </summary>
        Task<IReadOnlyList<ProductPart>> LoadPartsAsync(Operation operation, IOperationLogger operationLogger, CancellationToken cancellationToken);
    }
}
