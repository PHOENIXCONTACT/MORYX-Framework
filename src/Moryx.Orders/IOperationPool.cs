// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
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
        Operation Get(Guid identifier);

        /// <summary>
        /// Will return the operation with the given order and operation numbers
        /// </summary>
        Operation Get(string orderNumber, string operationNumber);

        /// <summary>
        /// Will return all orders
        /// </summary>
        IReadOnlyList<Order> GetOrders();
    }
}