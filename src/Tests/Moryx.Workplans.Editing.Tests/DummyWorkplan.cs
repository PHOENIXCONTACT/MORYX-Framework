// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Workplans;
using Moryx.Workplans.WorkplanSteps;
using System.Drawing;
using System.Linq;

namespace Moryx.Tests.Workplans
{
    /// <summary>
    /// WorkplanDummy for tests
    /// </summary>
    public class WorkplanDummy : Workplan
    {
        /// <summary>
        /// Start -> In;
        /// Out 0 -> B;
        /// Out 1 -> C;
        /// </summary>
        public DummyStep StepA { get; private set; }
        /// <summary>
        /// A   -> In;
        /// Out -> C;
        /// </summary>
        public DummyStep StepB { get; private set; }
        /// <summary>
        /// A -> In;
        /// B -> In;
        /// Out 0 -> End;
        /// Out 1 -> End;
        /// </summary>
        public DummyStep StepC { get; private set; }

        public IConnector StartConnector { get; private set; }
        public IConnector EndConnector { get; private set; }

        // Workplan layout
        // Start -> A
        // A -> B
        // A -> C
        // B -> C
        // C -> End
        public WorkplanDummy()
        {
            Id = 1;
            Name = "DummyWorkplan";

            StartConnector = WorkplanInstance.CreateConnector("Start", NodeClassification.Start);
            EndConnector = WorkplanInstance.CreateConnector("End", NodeClassification.End);
            this.Add(StartConnector, EndConnector);

            StepA = new DummyStep(2, "A");
            StepA.OutputDescriptions = new[] { new OutputDescription { Name = "succeed" }, new OutputDescription { Name = "Failed" } };
            StepA.Inputs[0] = StartConnector;
            StepA.Position = new Point(1, 1);
            this.Add(StepA);

            var left = WorkplanInstance.CreateConnector("Left");
            left.Position = new Point(0, 2);
            this.Add(left);
            StepA.Outputs[0] = left;
            var right = WorkplanInstance.CreateConnector("Right");
            right.Position = new Point(2, 2);
            this.Add(right);
            StepA.Outputs[1] = right;

            StepB = new DummyStep(1, "B");
            StepB.Inputs[0] = right;
            StepB.Position = new Point(2, 3);
            this.Add(StepB);
            StepB.Outputs[0] = left;

            StepC = new DummyStep(2, "C");
            StepC.OutputDescriptions = new[] { new OutputDescription { Name = "succeed" }, new OutputDescription { Name = "Failed" } };
            StepC.Inputs[0] = left;
            StepC.Position = new Point(1, 4);
            this.Add(StepC);
            StepC.Outputs[0] = StepC.Outputs[1] = EndConnector;
        }
    }
}
