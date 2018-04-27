using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;

namespace Marvin.Resources.Samples.Initializer
{
    [ResourceInitializer(nameof(SampleMachineInitializer))]
    public class SampleMachineInitializer : ResourceInitializerBase
    {
        public override string Name => "Sample Machine";

        public override string Description => "Creates a sample machine and two cells";

        public override IReadOnlyList<Resource> Execute(IResourceCreator creator)
        {
            var machine = creator.Instantiate<Machine>();

            var someGate = creator.Instantiate<GateResource>();
            someGate.Name = "Some Gate";

            someGate.Parent = machine;
            machine.Children.Add(someGate);

            var anotherGate = creator.Instantiate<GateResource>();
            anotherGate.Name = "Another Gate";

            anotherGate.Parent = machine;
            machine.Children.Add(anotherGate);

            return new Resource[] {machine};
        }
    }
}
