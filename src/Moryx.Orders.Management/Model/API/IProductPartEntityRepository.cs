﻿#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.Orders.Management.Model
{
    public interface IProductPartEntityRepository : IRepository<ProductPartEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        ProductPartEntity Create(string description, string number, double quantity, string unit);
    }
}
