// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be used to validate the complete operation after finishing the creation
    /// </summary>
    public interface IOperationValidation : IAsyncConfiguredPlugin<OperationValidationConfig>
    {
        /// <summary>
        /// Validates the given operation
        /// </summary>
        Task<bool> ValidateAsync(Operation operation, IOperationLogger operationLogger, CancellationToken cancellationToken);

        /// <summary>
        /// Validates the given creation context
        /// </summary>
        Task<bool> ValidateCreationContextAsync(OrderCreationContext orderContext, CancellationToken cancellationToken);
    }
}
