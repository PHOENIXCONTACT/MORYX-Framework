// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Resources;
using Moryx.ControlSystem.VisualInstructions;
using Moryx.Logging;
using NUnit.Framework;

namespace Moryx.ControlSystem.WorkerSupport.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        private Mock<IVisualInstructionSource> _instructor;

        private WorkerSupportController _controller;

        [SetUp]
        public void PrepareSystem()
        {
            var resourceManagement = new Mock<IResourceManagement>();

            _instructor = new Mock<IVisualInstructionSource>();
            _instructor.SetupGet(i => i.Identifier).Returns("Foo");

            resourceManagement.Setup(rm => rm.GetResources<IVisualInstructionSource>())
                .Returns([_instructor.Object]);

            _controller = new WorkerSupportController
            {
                Logger = new Mock<IModuleLogger>().Object,
                ResourceManagement = resourceManagement.Object,
                Config = new ModuleConfig
                {
                    ProcessorConfigs = []
                }
            };
            _controller.InstructionAdded += EmptyListener;
            _controller.InstructionCleared += EmptyListener;
            _controller.Start();
        }

        private void EmptyListener(object sender, InstructionEventArgs args) { }

        [Test]
        public void ReturnAllInstructors()
        {
            // Arrange
            _controller.AddInstruction("Bla", new ActiveInstruction { Id = 42 });
            _controller.AddInstruction("Foo", new ActiveInstruction { Id = 43 }); // Same identifier as the resource

            // Act
            var instructors = _controller.GetInstructors();

            // Assert
            Assert.That(instructors, Is.Not.Null);
            Assert.That(instructors.Count, Is.EqualTo(2));
        }

        [Test]
        public void ForwardInstructionsToFacade()
        {
            // Arrange
            InstructionEventArgs added = null, cleared = null;
            _controller.InstructionAdded += (sender, args) => added = args;
            _controller.InstructionCleared += (sender, args) => cleared = args;

            // Act
            _instructor.Raise(i => i.Added += null, _instructor.Object, new ActiveInstruction { Id = 42 });
            _instructor.Raise(i => i.Cleared += null, _instructor.Object, new ActiveInstruction { Id = 42 });

            // Assert
            Assert.That(added, Is.Not.Null);
            Assert.That(cleared, Is.Not.Null);
            Assert.That(added.Instruction.Id, Is.EqualTo(42));
            Assert.That(cleared.Instruction.Id, Is.EqualTo(42));
        }

        [Test]
        public void ForwardMethodsToResource()
        {
            // Arrange
            var instruction = new ActiveInstruction { Id = 42 };
            _instructor.SetupGet(i => i.Instructions).Returns([instruction]);

            // Act
            var response = new ActiveInstructionResponse { Id = 42, SelectedResult = new InstructionResult { Key = "Ok" } };
            _controller.CompleteInstruction("Foo", response);

            // Assert
            _instructor.Verify(rm => rm.Completed(response), Times.Once);
        }
    }
}

