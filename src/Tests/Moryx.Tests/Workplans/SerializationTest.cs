// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Drawing;
using System.Linq;
using Moryx.Serialization;
using Moryx.Workplans;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Moryx.Tests.Workplans
{
    [TestFixture]
    public class SerializationTest
    {
        [Test]
        public void JsonCycle()
        {
            // Arrange
            var workplan = new Workplan();
            var start = WorkplanInstance.CreateConnector("Initial", NodeClassification.Start);
            var step = new DummyStep(1, "Test");
            var end = WorkplanInstance.CreateConnector("End", NodeClassification.End);
            step.Inputs[0] = start;
            step.Outputs[0] = end;
            step.Position = new Point(1, 1);
            workplan.Add(start, end);
            workplan.Add(step);

            // Act
            var json = JsonConvert.SerializeObject(workplan, JsonSettings.Minimal);
            var deserialized = JsonConvert.DeserializeObject<Workplan>(json, JsonSettings.Minimal);

            // Assert
            var connectors = workplan.Connectors.ToList();
            Assert.That(deserialized.Connectors.Count(), Is.EqualTo(connectors.Count));
            Assert.That(deserialized.Connectors.First().Name, Is.EqualTo(connectors[0].Name));
            Assert.That(deserialized.Connectors.ElementAt(1).Classification, Is.EqualTo(connectors[1].Classification));
            Assert.That(deserialized.Steps.Count(), Is.EqualTo(workplan.Steps.Count()));
            Assert.That(deserialized.Steps.First(), Is.InstanceOf<DummyStep>());
            Assert.That(deserialized.Steps.First().Name, Is.EqualTo(workplan.Steps.First().Name));
            Assert.That(deserialized.Steps.First().Position, Is.EqualTo(workplan.Steps.First().Position));
        }
    }
}
