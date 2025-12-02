// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.AbstractionLayer.Processes;
using Moryx.ControlSystem.Activities;
using Moryx.ControlSystem.Cells;
using Moryx.ControlSystem.TestTools.Activities;
using Moryx.Logging;
using Moryx.Resources.AssemblyInstruction;
using Moryx.Threading;
using NUnit.Framework;

namespace Moryx.ControlSystem.VisualInstructions.Tests
{
    [TestFixture]
    public class VisualInstructorTests
    {
        private VisualInstructor _instructor;

        private const string ClientId1 = "Client1";
        private const string ResourceName = "UnitTest";

        [SetUp]
        public void Setup()
        {
            var logger = new ModuleLogger("Dummy", new NullLoggerFactory(), (l, s, e) => { });
            _instructor = new VisualInstructor
            {
                Name = ClientId1,
                Logger = logger,
                ParallelOperations = new ParallelOperations(logger)
            };
        }

        [Test]
        public void Display()
        {
            // Arrange
            var wasRaised = false;
            _instructor.Added += (sender, args) => wasRaised = true;

            // Act
            var id = _instructor.Display(new ActiveInstruction
            {
                Title = ResourceName,
                Instructions = GetTextInstruction()
            });

            // Assert
            Assert.That(wasRaised);
            Assert.That(_instructor.Instructions.Count, Is.EqualTo(1));
            Assert.That(_instructor.Instructions[0].Id, Is.EqualTo(id));
            Assert.That(_instructor.CurrentInstructions, Is.EqualTo($"{id}"));
        }

        [Test]
        public void ClearDisplayed()
        {
            // Arrange
            var instructionId = _instructor.Display(new ActiveInstruction
            {
                Title = ResourceName,
                Instructions = GetTextInstruction()
            });
            var wasRaised = false;
            _instructor.Cleared += (sender, args) => wasRaised = true;

            // Act
            _instructor.Clear(instructionId);

            // Assert
            Assert.That(wasRaised);
            Assert.That(_instructor.Instructions?.Count, Is.EqualTo(0));
            Assert.That(_instructor.CurrentInstructions, Is.EqualTo(""));
            Assert.DoesNotThrow(() => _instructor.Clear(instructionId));
        }

        [Test]
        public void DisplayAutoClear()
        {
            // Arrange
            var addWasRaised = false;
            var clearWasRaised = false;
            _instructor.Added += (sender, args) => addWasRaised = true;
            _instructor.Cleared += (sender, args) => clearWasRaised = true;

            // Act
            _instructor.Display(new ActiveInstruction
            {
                Title = ResourceName,
                Instructions = GetTextInstruction()
            }, 100);
            Thread.Sleep(150);

            // Assert
            Assert.That(addWasRaised);
            Assert.That(clearWasRaised);
            Assert.That(_instructor.Instructions?.Count, Is.EqualTo(0));
            Assert.That(_instructor.CurrentInstructions, Is.EqualTo(""));
        }

        [Test]
        public void Execute()
        {
            // Arrange
            var callbackRaised = false;
            var callbackResult = string.Empty;
            const string defaultResult = "0";

            // Act
            var instructionId = _instructor.Execute(new ActiveInstruction
            {
                Title = ResourceName,
                Instructions = GetTextInstruction(),
                Results = [new InstructionResult { Key = defaultResult }]
            }, delegate (ActiveInstructionResponse response)
            {
                callbackRaised = true;
                callbackResult = response.SelectedResult.Key;
            });

            _instructor.Completed(new ActiveInstructionResponse { Id = instructionId, SelectedResult = new InstructionResult { Key = defaultResult } });

            // Assert
            Assert.That(callbackRaised);
            Assert.That(callbackResult, Is.EqualTo(defaultResult));
        }

        [Test]
        public void ActivityExecute()
        {
            // Arrange
            var activityStart = CreateActivityStart();
            ActivityStart callbackActivityStart = null;
            var callbackRaised = false;
            var callbackResult = 0;
            var eventRaised = false;
            _instructor.Cleared += (sender, args) => eventRaised = true;

            // Act
            var instructionId = _instructor.Execute(ResourceName, activityStart,
                delegate (int result, ActivityStart origin)
                {
                    callbackRaised = true;
                    callbackResult = result;
                    callbackActivityStart = origin;
                });

            _instructor.Completed(new ActiveInstructionResponse { Id = instructionId, SelectedResult = new InstructionResult { Key = "0" } });

            // Assert
            Assert.That(callbackResult, Is.EqualTo((int)MountingResult.Mounted));
            Assert.That(callbackRaised);
            Assert.That(callbackActivityStart, Is.EqualTo(activityStart));
            Assert.That(eventRaised);
        }

        [Test]
        public void DisplayActivity()
        {
            // Arrange
            var activityStart = CreateActivityStart();
            var wasRaised = false;
            _instructor.Added += (sender, args) => wasRaised = true;

            // Act
            var id = _instructor.Display(ResourceName, activityStart);

            // Assert
            Assert.That(wasRaised);
            Assert.That(_instructor.Instructions.Count, Is.EqualTo(1));
            Assert.That(_instructor.Instructions[0].Id, Is.EqualTo(id));
            Assert.That(_instructor.CurrentInstructions, Is.EqualTo($"{id}"));
        }

        private static ActivityStart CreateActivityStart()
        {
            var activity = new MountActivity
            {
                Process = new Process { Id = 4711 },
                Parameters = new MountingParameters
                {
                    Instructions =
                    [
                        new VisualInstruction {Content = "Hello World", Type = InstructionContentType.Text}
                    ]
                }
            };

            var session = Session.StartSession(ActivityClassification.Production, ReadyToWorkType.Push);
            return session.StartActivity(activity);
        }

        private static VisualInstruction[] GetTextInstruction()
        {
            return
            [
                new VisualInstruction {Content = "Hello World", Type = InstructionContentType.Text}
            ];
        }
    }
}
