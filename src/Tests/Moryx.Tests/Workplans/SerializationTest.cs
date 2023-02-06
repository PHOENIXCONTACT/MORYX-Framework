// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
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
            Assert.AreEqual(connectors.Count, deserialized.Connectors.Count());
            Assert.AreEqual(connectors[0].Name, deserialized.Connectors.First().Name);
            Assert.AreEqual(connectors[1].Classification, deserialized.Connectors.ElementAt(1).Classification);
            Assert.AreEqual(workplan.Steps.Count(), deserialized.Steps.Count());
            Assert.IsInstanceOf<DummyStep>(deserialized.Steps.First());
            Assert.AreEqual(workplan.Steps.First().Name, deserialized.Steps.First().Name);
            Assert.AreEqual(workplan.Steps.First().Position, deserialized.Steps.First().Position);
        }
    }
}
