// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using NUnit.Framework;

namespace Moryx.VisualInstructions.Endpoints.Tests
{
    [TestFixture]
    public class VisualInstructionsControllerTests
    {
        private VisualInstructionsFacadeMock _visualInstructionsFacadeMock;
        private VisualInstructionsController _controller;

        [SetUp]
        public void SetUp()
        {

            _visualInstructionsFacadeMock = new VisualInstructionsFacadeMock();
            _controller = new VisualInstructionsController(_visualInstructionsFacadeMock);
        }

        [Test]
        [TestCase("Instructor")]
        [TestCase("Ümlaut")]
        [TestCase("Instructor with spaces")]
        public void ShouldReturnInstructions(string identifier)
        {
            // Act
            var result = _controller.GetAll(identifier);

            // Assert
            Assert.That(result.Value, Is.Not.Empty);
        }

        [Test]
        public void ShouldReturnEmptyList()
        {
            // Act
            var result = _controller.GetAll("NotExisting");

            // Assert
            Assert.That(result.Value, Is.Empty);
        }

        [Test]
        [TestCase("Instructor")]
        [TestCase("Ümlaut")]
        [TestCase("Instructor with spaces")]
        public void ShouldAddInstruction(string identifier)
        {
            // Arrange
            var id = new Random().Next();
            var instruction = EmptyInstruction(id);

            // Act
            _controller.AddInstruction(identifier, instruction);

            // Assert
            var result = _controller.GetAll(identifier);
            Assert.That(result.Value.Last().Id, Is.EqualTo(instruction.Id));
        }

        [Test]
        [TestCase("Instructor")]
        [TestCase("Ümlaut")]
        [TestCase("Instructor with spaces")]
        public void ShouldClearInstruction(string identifier)
        {
            // Arrange
            var id = new Random().Next();
            var instruction = EmptyInstruction(id);

            // Act
            _controller.AddInstruction(identifier, instruction);
            _controller.ClearInstruction(identifier, instruction);

            // Assert
            var result = _controller.GetAll(identifier);
            Assert.That(result.Value.Any(i => i.Id == instruction.Id), Is.False);
        }

        [Test]
        [TestCase("Instructor")]
        [TestCase("Ümlaut")]
        [TestCase("Instructor with spaces")]
        public void ShouldCompleteInstruction(string identifier)
        {
            // Arrange
            var id = new Random().Next();
            var instruction = EmptyInstruction(id);

            // Act
            _controller.AddInstruction(identifier, instruction);
            _controller.CompleteInstruction(identifier, new InstructionResponseModel { Id = id, Result = identifier });

            // Assert
            var result = _controller.GetAll(identifier);
            Assert.That(result.Value.Any(i => i.Id == instruction.Id), Is.False);
            Assert.That(_visualInstructionsFacadeMock.CompleteResult, Is.EqualTo(identifier));
        }

        [Test]
        [TestCase("Instructor")]
        [TestCase("Ümlaut")]
        [TestCase("Instructor with spaces")]
        public void ShouldCompleteInstructionWithInputs(string identifier)
        {
            // Arrange
            var id = new Random().Next();
            var instruction = EmptyInstruction(id);

            // Act
            _controller.AddInstruction(identifier, instruction);
            _controller.CompleteInstruction(identifier, new InstructionResponseModel { Id = id, Result = identifier });

            // Assert
            var result = _controller.GetAll(identifier);
            Assert.That(result.Value.Any(i => i.Id == instruction.Id), Is.False);
            Assert.That(_visualInstructionsFacadeMock.CompleteResult, Is.EqualTo(identifier));
        }

        private static InstructionModel EmptyInstruction(long id)
        {
            return new InstructionModel
            {
                Id = id,
                Items = [],
            };
        }
    }
}

