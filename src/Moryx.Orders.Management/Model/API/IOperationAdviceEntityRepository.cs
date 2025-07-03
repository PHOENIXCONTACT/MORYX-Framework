#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.Orders.Management.Model
{
    public interface IOperationAdviceEntityRepository : IRepository<OperationAdviceEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        OperationAdviceEntity Create(string loadingEquipment, int amount);
    }
}
