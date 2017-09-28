using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleADependend : ModuleBase
    {
        [RequiredModuleApi]
        public IFacadeA Dependency { get; set; }
    }
}