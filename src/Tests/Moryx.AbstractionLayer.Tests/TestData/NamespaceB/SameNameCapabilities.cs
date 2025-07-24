using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.AbstractionLayer.Resources.TestData.NamespaceB
{
    public class SameNameCapabilities : CapabilitiesBase
    {
        protected override bool ProvidedBy(ICapabilities provided)
        {
            return true;
        }
    }
}
