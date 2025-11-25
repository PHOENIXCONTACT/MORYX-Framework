// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    [Description("Cell to mount product instances on carriers!")]
    [ResourceRegistration]
    public class MountingCell : Cell
    {
        [DataMember, EntrySerialize, DefaultValue("Bye bye, WPC!")]
        public string OutfeedMessage { get; set; }

        [ResourceReference(ResourceRelationType.Driver)]
        public IMessageDriver Driver { get; set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Driver.Received += new HandlerMap<object>($"{Id}-{Name}")
                .Register<object>(OnPlcMessage)
                .ReceivedHandler;
        }

        private static void OnPlcMessage(object sender, object message)
        {
            // Handler that also uses the sender
        }

        [EntrySerialize]
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

    public class ManualMountingCell : MountingCell
    {

        // Automatically detect relation name based
        public IVisualInstructor Instructor { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangeablePart, AutoSave = true)]
        public IReferences<IWpc> CurentWpcs { get; set; }
    }
}
