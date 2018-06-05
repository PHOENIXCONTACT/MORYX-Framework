using Marvin.Runtime.Modules;

namespace Marvin.Runtime.Kernel.Tests.ModuleMocks
{
    internal class ModuleA : ModuleBase, IFacadeContainer<IFacadeA>
    {
        public ModuleA()
        {
            Facade = new FacadaA();
        }

        /// <summary>
        /// Facade controlled by this module
        /// </summary>
        /// <remarks>
        /// The hard-coded name of this property is also used in Marvin.Runtime.Kernel\ModuleManagement\Components\ModuleDependencyManager.cs
        /// </remarks>
        public IFacadeA Facade { get; private set; }
    }

    internal class FacadaA : IFacadeA
    {
    }
}