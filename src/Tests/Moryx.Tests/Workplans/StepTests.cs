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
            Assert.That(transition, Is.InstanceOf<SplitTransition>());
            Assert.That(step.Outputs.Length, Is.EqualTo(2));
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
            Assert.That(transition, Is.InstanceOf<JoinTransition>());
            Assert.That(step.Outputs.Length, Is.EqualTo(1));
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
            Assert.That(trans, Is.InstanceOf<SubworkplanTransition>());
            Assert.That(step.Outputs.Length, Is.EqualTo(2));
            Assert.That(step.OutputDescriptions.Length, Is.EqualTo(2));
            var success = step.OutputDescriptions[0];
            Assert.That(success.Name, Is.EqualTo("End"));
            Assert.That(success.OutputType, Is.EqualTo(OutputType.Success));
            Assert.That(success.MappingValue, Is.EqualTo(exits[0].Id));
            var failed = step.OutputDescriptions[1];
            Assert.That(failed.Name, Is.EqualTo("Failed"));
            Assert.That(failed.OutputType, Is.EqualTo(OutputType.Failure));
            Assert.That(failed.MappingValue, Is.EqualTo(exits[1].Id));
        }
    }
}
