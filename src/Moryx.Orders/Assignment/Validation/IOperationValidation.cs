// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moryx.Modules;

namespace Moryx.Orders.Assignment
{
    /// <summary>
    /// Will be used to validate the complete operation after finishing the creation
    /// </summary>
    public interface IOperationValidation : IConfiguredPlugin<OperationValidationConfig>
    {
        /// <summary>
        /// Validates the given operation
        /// </summary>
        Task<bool> Validate(Operation operation, IOperationLogger operationLogger);

        /// <summary>
        /// Validates the given creation context
        /// </summary>
        bool ValidateCreationContext(OrderCreationContext orderContext);
    }
}