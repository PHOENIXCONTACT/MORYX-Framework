// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Interface to provide operation depending logging
    /// </summary>
    internal interface IOperationLoggerProvider
    {
        /// <summary>
        /// Gets the creation logger for the operation
        /// </summary>
        IOperationLogger GetLogger(IOperationData operationData);

        /// <summary>
        /// Removes a logger for the given operation
        /// </summary>
        void RemoveLogger(IOperationData operationData);
    }
}
