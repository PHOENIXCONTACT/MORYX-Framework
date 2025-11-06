// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.AbstractionLayer;
using Moryx.AbstractionLayer.Activities;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.ProcessEngine.Jobs;
using Moryx.ControlSystem.TestTools;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.ControlSystem.TestTools.Tasks;
using NUnit.Framework;

namespace Moryx.ControlSystem.ProcessEngine.Tests.Jobs
{
    [TestFixture]
    public class WorkplanValidationTests
    {
        [Test]
        public void DetectOpenOutput()
        {
            // Arrange
            var dummy = DummyRecipe.BuildRecipe(1);
            // Break workplan
            dummy.Workplan.Steps.ElementAt(1).Outputs[1] = null;

            // Act
            var evaluation = WorkplanValidation.Validate(dummy.Workplan);

            // Assert
            Assert.That(evaluation.Count, Is.EqualTo(1));
        }

        [Test]
        public void DetectInvalidParameter()
        {
            // Arrange
            var dummy = DummyRecipe.BuildRecipe(1);
            var step = (AssignIdentityTask)dummy.Workplan.Steps.ElementAt(1);
            step.Parameters = new ValidationParameters();

            // Act
            var evaluation = WorkplanValidation.Validate(dummy.Workplan);

            // Assert
            Assert.That(evaluation.Count, Is.EqualTo(1));
        }

        private class ValidationParameters : AssignIdentityParameters
        {
            [System.ComponentModel.DataAnnotations.Range(0, 7)]
            public int Invalid { get; set; } = 42;

            protected override void Populate(IProcess process, Parameters instance)
            {
            }
        }
    }
}
