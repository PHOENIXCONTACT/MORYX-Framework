// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using Moryx.Workplans;
using Moryx.Workplans.Transitions;
using Moryx.Workplans.WorkplanSteps;
using NUnit.Framework;

namespace Moryx.Tests.Workplans
{
    [TestFixture]
    public class StepTests
    {
        [Test]
        public void SplitStepNormal()
        {
            // Arrange
            var step = new SplitWorkplanStep();

            // Act
            var transition = step.CreateInstance(null);

            // Assert
            Assert.IsInstanceOf<SplitTransition>(transition);
            Assert.AreEqual(2, step.Outputs.Length);
        }

        [Test]
        public void SingleSplit()
        {
            // Assert
            Assert.Throws<ArgumentException>(() => new SplitWorkplanStep(1));
        }

        [Test]
        public void JoinStep()
        {
            // Arrange
            var step = new JoinWorkplanStep();

            // Act
            var transition = step.CreateInstance(null);

            // Assert
            Assert.IsInstanceOf<JoinTransition>(transition);
            Assert.AreEqual(1, step.Outputs.Length);
        }

        [Test]
        public void SubWorkplan()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateSub();
            var step = new SubworkplanStep(workplan);
            var exits = workplan.Connectors.Where(c => c.Classification.HasFlag(NodeClassification.Exit)).ToArray();

            // Act
            var trans = step.CreateInstance(null);

            // Assert
            Assert.IsInstanceOf<SubworkplanTransition>(trans);
            Assert.AreEqual(2, step.Outputs.Length);
            Assert.AreEqual(2, step.OutputDescriptions.Length);
            var success = step.OutputDescriptions[0];
            Assert.AreEqual("End", success.Name);
            Assert.AreEqual(OutputType.Success, success.OutputType);
            Assert.AreEqual(exits[0].Id, success.MappingValue);
            var failed = step.OutputDescriptions[1];
            Assert.AreEqual("Failed", failed.Name);
            Assert.AreEqual(OutputType.Failure, failed.OutputType);
            Assert.AreEqual(exits[1].Id, failed.MappingValue);
        }
    }
}
