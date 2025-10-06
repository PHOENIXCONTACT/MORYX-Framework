// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Workplans;
using Moryx.Workplans.Validation;
using NUnit.Framework;

namespace Moryx.Tests.Workplans
{
    [TestFixture]
    public class EngineTests
    {
        private bool _completed;

        [Test]
        public void InstantiateWorkplan()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateFull();

            // Act
            var context = new FakeContext();
            var workplanInstance = WorkplanInstanceFactory.Instantiate(workplan, context);

            // Simple assert
            Assert.That(workplan.Connectors.Count(), Is.EqualTo(workplanInstance.Places.Count()), "Not all connectors transformed to places!");
            Assert.That(workplan.Steps.Count(), Is.EqualTo(workplanInstance.Transitions.Count()), "Not all steps transformed to transitions!");
            Assert.That(workplanInstance.Transitions.Cast<DummyTransition>().All(t => t.Context == context), "Context not passed to all transitions!");
            // Structure assert
            var transitions = workplanInstance.Transitions;
            Assert.That(transitions[0].Outputs.Length, Is.EqualTo(2));
            Assert.That(transitions[1].Inputs[0], Is.EqualTo(transitions[0].Outputs[1]));
            Assert.That(transitions[2].Inputs[0], Is.EqualTo(transitions[0].Outputs[0]));
            Assert.That(transitions[2].Inputs[0], Is.EqualTo(transitions[1].Outputs[0]));
            Assert.That(transitions[2].Outputs[1], Is.EqualTo(transitions[2].Outputs[0]));
            Assert.That(workplanInstance.EndPlaces().First(), Is.EqualTo(transitions[2].Outputs[0]));
        }

        private class FakeContext : IWorkplanContext
        {
            public bool IsDisabled(IWorkplanStep step)
            {
                return step.Id == 42;
            }
        }

        [Test]
        public void ValidateFullSuccess()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateFull();

            // Act
            var validation = WorkplanInstance.Validate(workplan, ValidationAspect.DeadEnd | ValidationAspect.LoneWolf);

            // Assert
            Assert.That(validation.Success, "Validation did not return success for a valid workplan!");
            Assert.That(validation.Errors.Length, Is.EqualTo(0), "Valid workplan must not report errors!");
        }

        [Test]
        public void ValidateLoneWolf()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateLoneWolf();

            // Act
            var validation = WorkplanInstance.Validate(workplan, ValidationAspect.LoneWolf);

            // Assert
            Assert.That(validation.Success, Is.False, "Validation did not detect error!");
            Assert.That(validation.Errors.Length, Is.EqualTo(1), "Validation should have found one error!");
            Assert.That(validation.Errors[0], Is.InstanceOf<LoneWolfValidationError>(), "Error should be of type \"LoneWolfValidationError\"");
            var expected = workplan.Steps.First(s => s.Name == "LoneWolf");
            Assert.That(validation.Errors[0].PositionId, Is.EqualTo(expected.Id));
            var error = validation.Errors[0].Print(workplan);
            Assert.That(error, Is.Not.Null);
        }

        [Test]
        public void ValidateDeadEnd()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateDeadEnd();

            // Act
            var validation = WorkplanInstance.Validate(workplan, ValidationAspect.DeadEnd);

            // Assert
            Assert.That(validation.Success, Is.False, "Validation did not detect error!");
            Assert.That(validation.Errors.Length, Is.EqualTo(1), "Validation should have found one error!");
            Assert.That(validation.Errors[0], Is.InstanceOf<DeadEndValidationError>(), "Error should be of type \"DeadEndValidationError\"");
            var expected = workplan.Connectors.First(c => c.Name == "DeadEnd");
            Assert.That(validation.Errors[0].PositionId, Is.EqualTo(expected.Id));
            var error = validation.Errors[0].Print(workplan);
            Assert.That(error, Is.Not.Null);
        }

        [TestCase(ExecutionPath.Default, 2, "->A->C", Description = "Executing workplan instance on default path")]
        [TestCase(ExecutionPath.Alternative, 3, "->A->B->C", Description = "Executing workplan instance on alternative path")]
        public void ExecuteWorkplanInstance(ExecutionPath route, int expectedTransitions, string expectedPath)
        {
            // Arrange
            var workplan = WorkplanDummy.CreateFull();
            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());

            // Act
            var triggerCount = 0;
            var path = string.Empty;
            engine.TransitionTriggered += delegate (object sender, ITransition transition)
            {
                var dummy = (DummyTransition)transition;
                if (route == ExecutionPath.Alternative)
                    dummy.ResultOutput = dummy.Outputs.Length - 1;
                triggerCount++;
                path += "->" + dummy.Name;
            };
            engine.Completed += EngineCompleted;
            engine.Start();
            // Synchronus execution means we are done here
            WorkplanInstance.Destroy(engine);

            // Assert
            Assert.That(_completed);
            Assert.That(triggerCount, Is.EqualTo(expectedTransitions), "Less transitions triggered than expected!");
            Assert.That(path, Is.EqualTo(expectedPath), "Workplan engine did not take the correct path!");
        }

        public enum ExecutionPath
        {
            Default,
            Alternative
        }

        [Test]
        public void PauseAndResume()
        {
            // Arrange
            var workplan = WorkplanDummy.CreatePausable();
            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());

            engine.TransitionTriggered += (sender, transition) => { };
            engine.Completed += EngineCompleted;

            // Act
            engine.Start(); // <-- This runs till the first pausable transition
            var snapShot = engine.Pause();
            // Once we resume it will continue
            engine.Start();

            // Assert
            var stepId = workplan.Steps.Single(s => s is PausableStep).Id;
            Assert.That(_completed);
            Assert.That(snapShot.Holders.Length, Is.EqualTo(1));
            Assert.That(snapShot.Holders[0].HolderId, Is.EqualTo(stepId));
            Assert.That(snapShot.Holders[0].HolderState, Is.InstanceOf<MainToken>());
            Assert.That(snapShot.Holders[0].Tokens.Length, Is.EqualTo(1));
        }

        [Test]
        public void RestoreAndResume()
        {
            // Arrange
            var workplan = WorkplanDummy.CreatePausable();
            var engine = WorkplanInstance.CreateEngine(workplan, new NullContext());
            var stepId = workplan.Steps.Single(s => s is PausableStep).Id;
            var snapShot = new WorkplanSnapshot
            {
                Holders =
                [
                    new HolderSnapshot
                    {
                        HolderId = stepId,
                        Tokens = [new MainToken()]
                    }
                ]
            };

            // Act
            engine.Restore(snapShot);
            engine.Completed += EngineCompleted;
            engine.TransitionTriggered += (sender, transition) => { };
            engine.Start(); // <-- This run to the end

            // Assert
            Assert.That(_completed);
        }

        private void EngineCompleted(object sender, IPlace place)
        {
            _completed = true;
        }
    }
}
