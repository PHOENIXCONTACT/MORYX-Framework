using System;
using Marvin.Runtime;
using Marvin.Runtime.Base;
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
