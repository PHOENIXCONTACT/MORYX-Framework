#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.Collections.Generic;
using System.Linq;
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public interface IActivityEntityRepository : IRepository<ActivityEntity>
    {
        /// <summary>
        /// Get running activities of a process
        /// </summary>
        IEnumerable<ActivityEntity> GetRunning(long processId);

        /// <summary>
        /// Get completed activities of a process
        /// </summary>
        IEnumerable<ActivityEntity> GetCompleted(long processId);
    }

    public abstract class ActivityEntityRepository : Repository<ActivityEntity>, IActivityEntityRepository
    {
        /// <inheritdoc />
        public IEnumerable<ActivityEntity> GetRunning(long processId) =>
            DbSet.Where(a => a.ProcessId == processId && a.Result == null);

        /// <inheritdoc />
        public IEnumerable<ActivityEntity> GetCompleted(long processId) =>
            DbSet.Where(a => a.ProcessId == processId && a.Result != null);
    }
}
