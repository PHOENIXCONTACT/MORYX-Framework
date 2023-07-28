using Moryx.AbstractionLayer.Capabilities;

namespace Moryx.Tests.Workplans.Dummies
{
    
        public class AssemblingCapabilities : CapabilitiesBase
        {
            public int Value { get; set; }

            protected override bool ProvidedBy(ICapabilities provided)
            {
                var providedAssembling = provided as AssemblingCapabilities;
                if (providedAssembling == null)
                    return false;

                if (providedAssembling.Value < Value) // Provided must be greater or equal
                    return false;

                return true;
            }
        }
    
}