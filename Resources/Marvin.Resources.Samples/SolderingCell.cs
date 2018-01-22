using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Marvin.AbstractionLayer;
using Marvin.AbstractionLayer.Resources;
using Marvin.Resources.Management;

namespace Marvin.Resources.Samples
{
    [ResourceRegistration(nameof(SolderingCell))]
    public class SolderingCell : Cell
    {
        [ReferenceOverride(nameof(Children))]
        public IReferences<IStation> Stations { get; set; }
    }

    public interface IStation : IResource
    {
        void ProcessActivity(IActivity sa);
    }

    public abstract class Station : Resource, IStation
    {
        public abstract void ProcessActivity(IActivity sa);
    }

    [ResourceRegistration(nameof(HeatingStation))]
    public class HeatingStation : Station
    {
        [ResourceReference(ResourceRelationType.TransportRoute)]
        public SolderingStation NextStation { get; set; }

        public override void ProcessActivity(IActivity sa)
        {
            NextStation?.ProcessActivity(sa);
        }
    }

    [ResourceRegistration(nameof(SolderingStation))]
    public class SolderingStation : Station
    {
        public override void ProcessActivity(IActivity sa)
        {
        }
    }
}