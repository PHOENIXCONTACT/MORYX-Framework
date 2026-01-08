// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Business object for order data
    /// </summary>
    internal class OrderData : IPersistentObject, IOrderData
    {
        private readonly List<IOperationData> _operations;

        /// <inheritdoc />
        public InternalOrder Order { get; }

        /// <inheritdoc />
        long IPersistentObject.Id { get; set; }

        /// <inheritdoc />
        public string Number => Order.Number;

        /// <inheritdoc />
        public IReadOnlyList<IOperationData> Operations => _operations;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrderData"/> class.
        /// </summary>
        public OrderData()
        {
            Order = new InternalOrder();
            _operations = new List<IOperationData>();
        }

        /// <inheritdoc />
        public void Initialize(OrderCreationContext creationContext)
        {
            Order.Number = creationContext.Number;
            Order.Type = creationContext.Type;
        }

        /// <inheritdoc />
        public void AddOperation(IOperationData operationData)
        {
            // Add operation to the internal list of operations
            _operations.Add(operationData);

            // Add operation to the public list of operations
            Order.Operations.Add(operationData.Operation);

            // Map public operation with public order with this order
            operationData.Operation.Order = Order;
        }

        /// <inheritdoc />
        public void RemoveOperation(IOperationData operationData)
        {
            // Remove operation from the internal list of operations
            _operations.Remove(operationData);

            // Remove operation from the public list of operations
            Order.Operations.Remove(operationData.Operation);

            // Remove mapping of public order
            operationData.Operation.Order = null;
        }
    }
}
