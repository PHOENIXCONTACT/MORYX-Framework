// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.AssemblyInstruction;


namespace Moryx.Resources.Benchmarking
{
    [ResourceInitializer(nameof(BenchmarkInitializer))]
    public class BenchmarkInitializer : ResourceInitializerBase
    {
        private const int CellCount = 30;

        public override string Name => nameof(BenchmarkInitializer);

        public override string Description => $"Creates a ring of {CellCount} Benchmark resources";

        public override IReadOnlyList<Resource> Execute(IResourceGraph graph)
        {
            // Create Reporter
            var reporter = graph.Instantiate<BenchmarkReporter>();
            reporter.Name = "Benchmark Reporter";
            reporter.StepId = 42;

            // Create VisualInstructor
            var instructor = graph.Instantiate<VisualInstructor>();
            instructor.Name = "moryx-client";

            // Reference to reporter
            reporter.VisualInstructor = instructor;


            // Create cells
            var instances = new BenchmarkResource[CellCount];
            for (var i = 0; i < CellCount; i++)
            {
                var instance = graph.Instantiate<BenchmarkResource>();
                instances[i] = instance;

                // Properties
                instance.Name = $"Cell {i + 1:D2}";
                instance.StepId = i + 1;

                // References
                instance.Parent = reporter;

                instance.VisualInstructor = instructor;

                reporter.Children.Add(instance);
            }

            return new Resource[] { reporter };
        }
    }
}

