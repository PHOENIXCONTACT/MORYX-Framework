using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Drivers.Plc;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [Description("Cell to mount articles on carriers!")]
    [ResourceRegistration(nameof(MountingCell))]
    public class MountingCell : Cell
    {
        [DataMember, EditorVisible, DefaultValue("Bye bye, WPC!")]
        public string OutfeedMessage { get; set; }

        [ResourceReference(ResourceRelationType.Driver)]
        public IPlcDriver Driver { get; set; }

        public override void Initialize()
        {
            base.Initialize();

            Driver.Received += new HandlerMap<IQuickCast>($"{Id}-{Name}")
                .Register<IQuickCast>(OnPlcMessage)
                .ReceivedHandler;
        }

        private static void OnPlcMessage(object sender, IQuickCast readyToWork)
        {
            // Handler that also uses the sender
        }

        [EditorVisible]
        public Dummy CreateDummy(int number, string name)
        {
            return new Dummy
            {
                Number = number * 2,
                Name = name + number
            };
        }
        public class Dummy
        {
            public int Number { get; set; }

            public string Name { get; set; }
        }
    }

    public class ManualMountingCel : MountingCell
    {

        // Automatically detect relation name based
        public IVisualInstructor Instructor { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart, AutoSave = true)]
        public IReferences<IWpc> CurentWpcs { get; set; }
    }
}