using System.Linq;
using Marvin.Container;

namespace Marvin.DependentTestModule
{
    public class WcfBaseImporterSubInitializer : ISubInitializer
    {
        public void Initialize(IContainer container)
        {
            if (!container.GetRegisteredImplementations(typeof(ISimpleHelloWorldWcfSvcMgrFactory)).Any())
                container.Register<ISimpleHelloWorldWcfSvcMgrFactory>();
        }
    }
}