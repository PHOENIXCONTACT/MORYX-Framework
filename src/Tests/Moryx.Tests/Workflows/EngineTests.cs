// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.Workflows;
using Moryx.Workflows.Validation;
using NUnit.Framework;

namespace Moryx.Tests.Workflows
{
    [TestFixture]
    public class EngineTests
    {
        private bool _completed;

        [Test]
        public void InstantiateWorkflow()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateFull();

            // Act
            var context = new FakeContext();
            var workflow = WorkflowFactory.Instantiate(workplan, context);

            // Simple assert
            Assert.AreEqual(workplan.Connectors.Count(), workflow.Places.Count(), "Not all connectors transformed to places!");
            Assert.AreEqual(workplan.Steps.Count(), workflow.Transitions.Count(), "Not all steps transformed to transitions!");
            Assert.IsTrue(workflow.Transitions.Cast<DummyTransition>().All(t => t.Context == context), "Context not passed to all transitions!");
            // Structure assert
            var transitions = workflow.Transitions;
            Assert.AreEqual(2, transitions[0].Outputs.Length);
            Assert.AreEqual(transitions[0].Outputs[1], transitions[1].Inputs[0]);
            Assert.AreEqual(transitions[0].Outputs[0], transitions[2].Inputs[0]);
            Assert.AreEqual(transitions[1].Outputs[0], transitions[2].Inputs[0]);
            Assert.AreEqual(transitions[2].Outputs[0], transitions[2].Outputs[1]);
            Assert.AreEqual(transitions[2].Outputs[0], workflow.EndPlaces().First());
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
            var validation = Workflow.Validate(workplan, ValidationAspect.DeadEnd | ValidationAspect.LoneWolf);

            // Assert
            Assert.IsTrue(validation.Success, "Validation did not return success for a valid workplan!");
            Assert.AreEqual(0, validation.Errors.Length, "Valid workplan must not report errors!");
        }

        [Test]
        public void ValidateLoneWolf()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateLoneWolf();

            // Act
            var validation = Workflow.Validate(workplan, ValidationAspect.LoneWolf);

            // Assert
            Assert.IsFalse(validation.Success, "Validation did not detect error!");
            Assert.AreEqual(1, validation.Errors.Length, "Validation should have found one error!");
            Assert.IsInstanceOf<LoneWolfValidationError>(validation.Errors[0], "Error should be of type \"LoneWolfValidationError\"");
            var expected = workplan.Steps.First(s => s.Name == "LoneWolf");
            Assert.AreEqual(expected.Id, validation.Errors[0].PositionId);
            var error = validation.Errors[0].Print(workplan);
            Assert.NotNull(error);
        }

        [Test]
        public void ValidateDeadEnd()
        {
            // Arrange
            var workplan = WorkplanDummy.CreateDeadEnd();

            // Act
            var validation = Workflow.Validate(workplan, ValidationAspect.DeadEnd);

            // Assert
            Assert.IsFalse(validation.Success, "Validation did not detect error!");
            Assert.AreEqual(1, validation.Errors.Length, "Validation should have found one error!");
            Assert.IsInstanceOf<DeadEndValidationError>(validation.Errors[0], "Error should be of type \"DeadEndValidationError\"");
            var expected = workplan.Connectors.First(c => c.Name == "DeadEnd");
            Assert.AreEqual(expected.Id, validation.Errors[0].PositionId);
            var error = validation.Errors[0].Print(workplan);
            Assert.NotNull(error);
        }

        [TestCase(ExecutionPath.Default, 2, "->A->C", Description = "Executing workflow on default path")]
        [TestCase(ExecutionPath.Alternative, 3, "->A->B->C", Description = "Executiong workflow on alternative path")]
        public void ExecuteWorkflow(ExecutionPath route, int expectedTransitions, string expectedPath)
        {
            // Arrange
            var workplan = WorkplanDummy.CreateFull();
            var engine = Workflow.CreateEngine(workplan, new NullContext());

            // Act
            var triggerCount = 0;
            var path = string.Empty;
            engine.TransitionTriggered += delegate(object sender, ITransition transition)
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
            Workflow.Destroy(engine);

            // Assert
            Assert.IsTrue(_completed);
            Assert.AreEqual(expectedTransitions, triggerCount, "Less transitions triggered than expected!");
            Assert.AreEqual(expectedPath, path, "Workflow engine did not take the correct path!");
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
            var engine = Workflow.CreateEngine(workplan, new NullContext());

            engine.TransitionTriggered += (sender, transition) => { };
            engine.Completed += EngineCompleted;

            // Act
            engine.Start(); // <-- This runs till the first pausable transition
            var snapShot = engine.Pause();
            // Once we resume it will continue
            engine.Start();

            // Assert
            var stepId = workplan.Steps.Single(s => s is PausableStep).Id;
            Assert.IsTrue(_completed);
            Assert.AreEqual(1, snapShot.Holders.Length);
            Assert.AreEqual(stepId, snapShot.Holders[0].HolderId);
            Assert.IsInstanceOf<MainToken>(snapShot.Holders[0].HolderState);
            Assert.AreEqual(1, snapShot.Holders[0].Tokens.Length);
        }

        [Test]
        public void RestoreAndResume()
        {
            // Arrange
            var workplan = WorkplanDummy.CreatePausable();
            var engine = Workflow.CreateEngine(workplan, new NullContext());
            var stepId = workplan.Steps.Single(s => s is PausableStep).Id;
            var snapShot = new WorkflowSnapshot
            {
                Holders = new[]
                {
                    new HolderSnapshot
                    {
                        HolderId = stepId,
                        Tokens = new IToken[] { new MainToken() }
                    }
                }
            };

            // Act
            engine.Restore(snapShot);
            engine.Completed += EngineCompleted;
            engine.TransitionTriggered += (sender, transition) => { };
            engine.Start(); // <-- This run to the end

            // Assert
            Assert.IsTrue(_completed);
        }

        private void EngineCompleted(object sender, IPlace place)
        {
            _completed = true;
        }
    }
}
