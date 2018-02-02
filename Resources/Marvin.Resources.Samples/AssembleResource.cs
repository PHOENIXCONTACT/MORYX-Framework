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

        [ResourceReference(ResourceRelationType.Extension)]
        public AssemleSetup Setup { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            if (Setup == null)
            {
                Setup = Creator.Instantiate<AssemleSetup>();
                RaiseResourceChanged();
            }
        }

        [EditorVisible]
        public void ChangeSetup(string orderNumber)
        {
            Setup.ChangeOrderNumber(orderNumber);
        }

        [EditorVisible]
        public class AssembleConfig
        {
            public string Name { get; set; }

            public int Number { get; set; }
        }
        
    }

    public class AssemleSetup : Resource
    {
        [DataMember]
        public string OrderNumber { get; set; }

        public void ChangeOrderNumber(string orderNumber)
        {
            OrderNumber = orderNumber;
            RaiseResourceChanged();
        }
    }
}