using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleADependend : ModuleBase
    {
        [RequiredModuleApi]
        public IFacadeA Dependency { get; set; }
    }
}