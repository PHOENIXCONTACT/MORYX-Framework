using Marvin.Model;

namespace Marvin.Products.Model
{
    /// <summary>
    /// The public API of the StepEntity repository.
    /// </summary>
    public interface IStepEntityRepository : IRepository<StepEntity>
    {
        /// <summary>
        /// Creates instance with all not nullable properties prefilled
        /// </summary>
        StepEntity Create(long stepId, string name, string assembly, string nameSpace, string classname); 
    }
}
