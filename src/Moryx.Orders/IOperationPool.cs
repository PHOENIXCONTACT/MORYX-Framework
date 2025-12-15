// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders
{
    /// <summary>
    /// Operation pool holds all operations
    /// </summary>
    public interface IOperationPool
    {
        /// <summary>
        /// Returns all operations which are added to the pool
        /// </summary>
        IReadOnlyList<Operation> GetAll();

        /// <summary>
        /// Returns all operation by a specific filter
        /// </summary>
        IReadOnlyList<Operation> GetAll(Func<Operation, bool> filter);

        /// <summary>
        /// Will return the operation with the given id
        /// </summary>
        Task<Operation> GetAsync(Guid identifier, CancellationToken cancellationToken = default);

        /// <summary>
        /// Will return the operation with the given order and operation numbers
        /// </summary>
        Task<Operation> GetAsync(string orderNumber, string operationNumber, CancellationToken cancellationToken = default);

        /// <summary>
        /// Will return all orders
        /// </summary>
        IReadOnlyList<Order> GetOrders();
    }
}
