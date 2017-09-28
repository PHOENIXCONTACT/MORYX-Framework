using Marvin.Runtime.ServerModules;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleB2 : ModuleBase, IFacadeContainer<IFacadeB>
    {
        public ModuleB2()
        {
            Facade = new FacadeB();
        }

        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        /// <remarks>
        /// The hard-coded name of this property is also used in Marvin.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
        /// </remarks>
        public IFacadeB Facade { get; private set; }
    }
}