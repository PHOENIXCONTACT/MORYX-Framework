// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using Marvin.AbstractionLayer.Resources;
using Marvin.Modules;

namespace Marvin.Resources.Samples.Initializer
{
    [ResourceInitializer(nameof(SampleMachineInitializer))]
    [ExpectedConfig(typeof(SampleMachineInitializerConfig))]
    public class SampleMachineInitializer : ResourceInitializerBase<SampleMachineInitializerConfig>
    {
        public override string Name => "Sample Machine";

        public override string Description => "Creates a sample machine and two cells";

        public override IReadOnlyList<Resource> Execute(IResourceGraph graph)
        {
            var machine = graph.Instantiate<Machine>();
            machine.Name = Config.MachineName;

            var someGate = graph.Instantiate<GateResource>();
            someGate.Name = "Some Gate";

            someGate.Parent = machine;
            machine.Children.Add(someGate);

            var anotherGate = graph.Instantiate<GateResource>();
            anotherGate.Name = "Another Gate";

            anotherGate.Parent = machine;
            machine.Children.Add(anotherGate);

            return new Resource[] { machine };
        }
    }
}
