// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.Resources.Samples
{
    [ResourceRegistration]
    public class SolderingCell : Cell
    {
        [ReferenceOverride(nameof(Children))]
        public IReferences<IStation> Stations { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart, "Pre")]
        public SolderingStation PreSoldering { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart, "Final")]
        public SolderingStation FinalSoldering { get; set; }

        [ResourceReference(ResourceRelationType.CurrentExchangablePart)]
        public HeatingStation Heating { get; set; }

        [ResourceReference(ResourceRelationType.PossibleExchangablePart)]
        public IReferences<IStation> EnabledStations { get; set; }
    }

    public interface IStation : IResource
    {
        void ProcessActivity(IActivity sa);
    }

    public abstract class Station : Resource, IStation
    {
        [ResourceReference(ResourceRelationType.CurrentExchangablePart, ResourceReferenceRole.Source)]
        public SolderingCell BackRef { get; set; }

        public abstract void ProcessActivity(IActivity sa);
    }

    [ResourceRegistration]
    public class HeatingStation : Station
    {
        [ResourceReference(ResourceRelationType.TransportRoute)]
        public SolderingStation NextStation { get; set; }

        public override void ProcessActivity(IActivity sa)
        {
            NextStation?.ProcessActivity(sa);
        }
    }

    [ResourceRegistration]
    public class SolderingStation : Station
    {
        public override void ProcessActivity(IActivity sa)
        {
        }
    }
}
