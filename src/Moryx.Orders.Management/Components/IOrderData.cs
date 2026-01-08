// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Interface for orders
    /// </summary>
    internal interface IOrderData
    {
        /// <summary>
        /// Public operation for this business object
        /// </summary>
        InternalOrder Order { get; }

        /// <summary>
        /// Number of the order
        /// </summary>
        string Number { get; }

        /// <summary>
        /// All available operations of the order
        /// </summary>
        IReadOnlyList<IOperationData> Operations { get; }

        /// <summary>
        /// Initializes the order data with the given creation context
        /// </summary>
        /// <param name="creationContext"></param>
        void Initialize(OrderCreationContext creationContext);

        /// <summary>
        /// Adds an operation to the given operation data
        /// </summary>
        /// <param name="operationData"></param>
        void AddOperation(IOperationData operationData);

        /// <summary>
        /// Removes an operation from the order data
        /// </summary>
        void RemoveOperation(IOperationData operationData);
    }
}
