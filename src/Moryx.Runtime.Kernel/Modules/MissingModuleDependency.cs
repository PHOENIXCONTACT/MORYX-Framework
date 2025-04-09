using Moryx.Runtime.Modules;

namespace Moryx.Runtime.Kernel.Modules
{
    /// <summary>
    /// Class representing a missing module dependency
    /// </summary>
    internal class MissingModuleDependency : IModuleDependency
    {
        public IServerModule RepresentedModule { get; private set; }

        public MissingModuleDependency(IServerModule representedModule)
        {
            RepresentedModule = representedModule;
        }

        public IReadOnlyList<IModuleDependency> Dependencies => throw new NotImplementedException();

        public IReadOnlyList<IModuleDependency> Dependends => throw new NotImplementedException();
    }
}
