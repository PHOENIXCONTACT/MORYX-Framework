using System;
using Marvin.Runtime.Modules;

namespace Marvin.DependentTestModule
{
    public class DependentTestModuleFacade : IFacadeControl, IDependentTestModule
    {
        public Action ValidateHealthState { get; set; }

        public void Activate()
        {
        }

        public void Deactivate()
        {
        }
    }
}
