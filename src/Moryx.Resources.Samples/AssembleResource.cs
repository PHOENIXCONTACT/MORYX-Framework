// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System.ComponentModel;
using System.Runtime.Serialization;
using Moryx.AbstractionLayer.Resources;
using Moryx.Serialization;

namespace Moryx.Resources.Samples
{
    [ResourceRegistration]
    [DisplayName("Handarbeitsplatz"), Description("Handarbeitsplatz für manuele Arbeiten.")]
    public class AssembleResource : Cell
    {
        [DataMember]
        public AssembleConfig Config { get; set; }

        [Description("AssembleFoo for handling setups")]
        [ResourceReference(ResourceRelationType.Extension)]
        public AssembleFoo Setup { get; set; }

        [ResourceReference(ResourceRelationType.Extension)]
        public IVisualInstructor Instructor { get; set; }

        protected override async Task OnInitializeAsync()
        {
            await base.OnInitializeAsync();

            if (Setup == null)
            {
                Setup = Graph.Instantiate<AssembleFoo>();
                RaiseResourceChanged();
            }
        }

        protected override Task OnStartAsync()
        {
            return base.OnStartAsync();
        }

        protected override Task OnStopAsync()
        {
            return base.OnStopAsync();
        }

        [EntrySerialize]
        public void ChangeNumber(string fooNumber)
        {
            Setup.ChangeNumber(fooNumber);
        }

        [EntrySerialize]
        public class AssembleConfig
        {
            public string Name { get; set; }

            public int Number { get; set; }
        }

    }

    [Description("Some cool assemble foo")]
    public class AssembleFoo : Resource
    {
        [DataMember]
        public string FooNumber { get; set; }

        public void ChangeNumber(string fooNumber)
        {
            FooNumber = fooNumber;
            RaiseResourceChanged();
        }
    }
}
