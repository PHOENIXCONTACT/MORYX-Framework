using Marvin.Container;
using Marvin.Model;

namespace Marvin.Runtime.Kernel
{
    /// <summary>
    /// Generic factory to provide access to all data models
    /// </summary>
    [KernelComponent(typeof(IModelResolver))]
    public class GenericUowFactory : IModelResolver
    {
        /// <summary>
        /// Global container which hold all data models. Injected by castle.
        /// </summary>
        public IContainer GlobalContainer { get; set; }

        /// <summary>
        /// Create an open context using the model namespace
        /// </summary>
        public IUnitOfWorkFactory GetByNamespace(string modelNamespace)
        {
            return GlobalContainer.Resolve<IUnitOfWorkFactory>(modelNamespace);
        }
    }
}
