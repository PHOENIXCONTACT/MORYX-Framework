using System.Runtime.Serialization;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Management;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration(nameof(AssembleResource))]
    public class AssembleResource : Cell
    {
        [DataMember]
        public AssembleConfig Config { get; set; }

        public override object Descriptor => Config;

        [EditorVisible]
        public class AssembleConfig
        {
            public string Name { get; set; }

            public int Number { get; set; }
        }
    }
}