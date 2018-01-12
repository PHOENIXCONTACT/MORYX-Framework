using System.Collections.Generic;
using Marvin.Model;

namespace Marvin.TestTools.Test.Model
{
    /// <summary>
    /// The public API of the HugePoco repository.
    /// </summary>
    public interface IHugePocoRepository : IRepository<HugePocoEntity>
    {
        HugePocoEntity Create(double float1, int number1, double float2, int number2, double float3, int number3, double float4, int number4, double float5, int number5);

        HugePocoEntity GetBy(double float1);

        HugePocoEntity GetBy(double float2, string name2, int number2);

        IEnumerable<HugePocoEntity> GetAllBy(double float3, string name3, int number3);
    }
}
