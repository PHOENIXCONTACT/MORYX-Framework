// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moryx.AbstractionLayer.Resources;

namespace Moryx.ControlSystem.VisualInstructions.Endpoints.Tests
{
    [TestFixture]
    public class WorkerSupportControllerTests
    {
        private WorkerSupportController _controller;
        private Mock<IResourceManagement> _resourceManagementMock;
        private Dictionary<string, IVisualInstructionSource> _instructors;
        private Mock<IVisualInstructionSource> _instructionSourceMock1;

        [SetUp]
        public void SetUp()
        {
            _resourceManagementMock = new Mock<IResourceManagement>();
            _controller = new WorkerSupportController(_resourceManagementMock.Object);

            _instructionSourceMock1 = new Mock<IVisualInstructionSource>();
            _instructionSourceMock1.SetupGet(i => i.Instructions).Returns([new ActiveInstruction()]);

            var instructionSourceMock2 = new Mock<IVisualInstructionSource>();
            instructionSourceMock2.SetupGet(i => i.Instructions).Returns([new ActiveInstruction()]);

            var instructionSourceMock3 = new Mock<IVisualInstructionSource>();
            instructionSourceMock3.SetupGet(i => i.Instructions).Returns([new ActiveInstruction()]);

            _instructors = new Dictionary<string, IVisualInstructionSource>
            {
                ["Instructor"] = _instructionSourceMock1.Object,
                ["Instructor with spaces"] = instructionSourceMock2.Object,
                ["Ümlaut"] = instructionSourceMock3.Object,
            };

            foreach (var instructorKvp in _instructors)
            {
                _resourceManagementMock.Setup(res => res.GetResource<IVisualInstructionSource>(instructorKvp.Key))
                    .Returns(instructorKvp.Value);
            }
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
            // Arrange
            const string identifier = "NotExisting";
            _resourceManagementMock.Setup(res => res.GetResource<IVisualInstructionSource>(identifier))
                .Returns((IVisualInstructionSource)null);

            // Act
            var result = _controller.GetAll(identifier);

            // Assert
            Assert.That(result.Result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public void ShouldCompleteInstruction()
        {
            // Arrange
            const string identifier = "Instructor";
            var instructionId = new Random().Next();
            var instruction = new ActiveInstruction { Id = instructionId };
            _instructionSourceMock1.SetupGet(i => i.Instructions).Returns([instruction]);

            ActiveInstructionResponse response = null;
            _instructionSourceMock1.Setup(i => i.Completed(It.IsAny<ActiveInstructionResponse>())).Callback((ActiveInstructionResponse rsp) =>
            {
                _instructionSourceMock1.SetupGet(i => i.Instructions).Returns([]);
                response = rsp;
            });

            // Act
            var completeResult = _controller.CompleteInstruction(identifier, new InstructionResponseModel { Id = instructionId, Result = identifier });

            // Assert
            Assert.That(completeResult, Is.Not.TypeOf<NotFoundObjectResult>());

            var getAllResult = _controller.GetAll(identifier);
            Assert.That(getAllResult.Value.Any(i => i.Id == instruction.Id), Is.False);
            Assert.That(response.SelectedResult.Key, Is.EqualTo(identifier));
        }
    }
}
