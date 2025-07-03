using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.ControlSystem.Capabilities
{
    /// <summary>
    /// Capabilities to assemble a product
    /// </summary>
    [DataContract]
    public class AssembleCapabilities : CapabilitiesBase
    {
        /// <inheritdoc />
        protected override bool ProvidedBy(ICapabilities provided)
        {
            return provided is AssembleCapabilities;
        }
    }
}