// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Logging;
using Moryx.Threading;
using Moryx.VisualInstructions;
using NUnit.Framework;

namespace Moryx.Resources.VisualInstructions.Tests
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

        private static VisualInstruction[] GetTextInstruction()
        {
            return
            [
                new VisualInstruction {Content = "Hello World", Type = InstructionContentType.Text}
            ];
        }
    }
}
