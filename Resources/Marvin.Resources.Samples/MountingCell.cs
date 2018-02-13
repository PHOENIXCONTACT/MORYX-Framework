using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Drivers.Plc;
using Marvin.AbstractionLayer.Identity;
using Marvin.AbstractionLayer.Resources;
using Marvin.Model;
using Marvin.Protocols.PhoenixPlc;
using Marvin.Resources.Management;
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
                .Register<ReadyToWorkMessage>(PlcReadyToWork)
                .Register<ProcessResultMessage>(ProcessResultReceived)
                .ReceivedHandler;
        }

        private void PlcReadyToWork(object sender, ReadyToWorkMessage readyToWork)
        {
            // Handler that also uses the sender
        }

        private void ProcessResultReceived(ProcessResultMessage obj)
        {
            // Handler that ignores the sender
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