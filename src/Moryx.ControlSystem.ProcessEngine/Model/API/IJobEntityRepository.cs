#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public interface IJobEntityRepository : IRepository<JobEntity>
    {
        IEnumerable<JobEntity> GetAllByState(int state);
    }
}
