using Marvin.Runtime.ModuleManagement;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleC : ModuleBase
    {
        [RequiredModuleApi]
        public IFacadeB[] Facades { get; set; }
    }
}