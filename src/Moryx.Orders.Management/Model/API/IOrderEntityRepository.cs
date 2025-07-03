#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.Orders.Management.Model
{
    public interface IOrderEntityRepository : IRepository<OrderEntity>
    {
        OrderEntity GetByNumber(string number);

        OrderEntity Create(string number);
    }
}
