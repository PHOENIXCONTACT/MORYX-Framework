using System;
using Marvin.Runtime;
using Marvin.Runtime.Modules;

namespace Marvin.TestModule
{
    public class TestModuleFacade : IFacadeControl, ITestModule
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
