// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Drivers.Plc;
using Marvin.AbstractionLayer.Resources;
using Marvin.Serialization;

namespace Marvin.Resources.Samples
{
    [Description("Cell to mount product instances on carriers!")]
    [ResourceRegistration]
    public class MountingCell : Cell
    {
        [DataMember, EditorBrowsable, DefaultValue("Bye bye, WPC!")]
        public string OutfeedMessage { get; set; }

        [ResourceReference(ResourceRelationType.Driver)]
        public IPlcDriver Driver { get; set; }

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

        [EditorBrowsable]
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

        [ResourceReference(ResourceRelationType.CurrentExchangablePart, AutoSave = true)]
        public IReferences<IWpc> CurentWpcs { get; set; }
    }
}
