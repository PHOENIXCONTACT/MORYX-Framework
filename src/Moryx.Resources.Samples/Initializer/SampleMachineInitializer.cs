// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Modules;

namespace Moryx.Resources.Samples.Initializer
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
            machine.AdditionalInformation = new MachineInfos
            {
                MaximumNumberOperator = 4,
                TechnicalStaff = "Max Mustermann",
            };
            machine.ProductionHours = 50000;
            machine.Description = "This is a description";
            machine.Values = new List<int> { 1, 2, 3, 4 };
            machine.PossibleTechnicalStaffs = new List<TechnicalStaff>()
            {
                new TechnicalStaff
                {
                    FirstName = "Max",
                    LastName = "Mustermann",
                    StaffNumber = 0815
                },
                new TechnicalStaff
                {
                    FirstName = "Flynn",
                    LastName = "Rider",
                    StaffNumber = 4711
                }
            };

            machine.MachineType = MachineType.halfAutomatic;
            machine.Power = 234.1;

            var someGate = graph.Instantiate<GateResource>();
            someGate.Name = "Some Gate";

            someGate.Parent = machine;
            machine.Children.Add(someGate);

            var anotherGate = graph.Instantiate<GateResource>();
            anotherGate.Name = "Another Gate";

            anotherGate.Parent = machine;
            machine.Children.Add(anotherGate);

            return [machine];
        }
    }
}
