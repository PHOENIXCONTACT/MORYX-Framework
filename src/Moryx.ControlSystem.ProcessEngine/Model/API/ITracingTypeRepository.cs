#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model.Repositories;

namespace Moryx.ControlSystem.ProcessEngine.Model
{
    public interface ITracingTypeRepository : IRepository<TracingType>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        TracingType Create(string assembly, string nameSpace, string classname);
    }
}
