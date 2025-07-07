// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Threading;
using System.Threading.Tasks;
using Moryx.Logging;
using Moq;
using Moryx.Orders.Management.Assignment;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationAssignmentTests
    {
        private OperationAssignment _assignment;
        private Mock<IOperationData> _operationDataMock;

        private Mock<IOperationAssignStep> _productAssignStepMock;
        private Mock<IOperationAssignStep> _partsAssignStepMock;
        private Mock<IOperationAssignStep> _recipeAssignStepMock;
        private Mock<IOperationAssignStep> _userAssignStepMock;
        private Mock<IOperationAssignStep> _validationStepMock;
        private Mock<IOperationAssignStep> _documentAssignStepMock;

        [SetUp]
        public void SetUp()
        {
            var orderMock = new Mock<IOrderData>();
            orderMock.SetupGet(o => o.Number).Returns("1001");

            _operationDataMock = new Mock<IOperationData>();
            _operationDataMock.SetupGet(o => o.OrderData).Returns(orderMock.Object);
            _operationDataMock.SetupGet(o => o.Number).Returns("1001");

            var stepFactoryMock = new Mock<IAssignStepFactory>();

            _productAssignStepMock = new Mock<IOperationAssignStep>();
            stepFactoryMock.Setup(f => f.Create(nameof(ProductAssignStep))).Returns(_productAssignStepMock.Object);

            _partsAssignStepMock = new Mock<IOperationAssignStep>();
            stepFactoryMock.Setup(f => f.Create(nameof(PartsAssignStep))).Returns(_partsAssignStepMock.Object);

            _recipeAssignStepMock = new Mock<IOperationAssignStep>();
            stepFactoryMock.Setup(f => f.Create(nameof(RecipeAssignStep))).Returns(_recipeAssignStepMock.Object);

            _userAssignStepMock = new Mock<IOperationAssignStep>();
            stepFactoryMock.Setup(f => f.Create(nameof(UserAssignStep))).Returns(_userAssignStepMock.Object);

            _documentAssignStepMock = new Mock<IOperationAssignStep>();
            stepFactoryMock.Setup(f => f.Create(nameof(DocumentAssignStep))).Returns(_documentAssignStepMock.Object);

            _validationStepMock = new Mock<IOperationAssignStep>();
            stepFactoryMock.Setup(f => f.Create(nameof(ValidateAssignStep))).Returns(_validationStepMock.Object);

            _assignment = new OperationAssignment
            {
                AssignStepFactory = stepFactoryMock.Object,
                LoggerProvider = new OperationLoggerProvider
                {
                    Logger = new ModuleLogger("Dummy", new NullLoggerFactory())
                }
            };
            _assignment.Start();
        }

        [TestCase(false, false, false, false, false, false)]
        [TestCase(true, false, false, false, false, false)]
        [TestCase(true, true, false, false, false, false)]
        [TestCase(true, true, true, false, false, false)]
        [TestCase(true, true, true, true, false, false)]
        [TestCase(true, true, true, true, true, true)]
        [Description("If some component in the creation chain fails, the creation result will be false")]
        public void CreateFailureIfChainFails(bool productResult, bool partsResult, bool recipeResult, bool documentResult, bool validationResult, bool expCreation)
        {
            // Arrange
            var createResetEvent = new ManualResetEvent(false);
            var creationResult = false;
            _operationDataMock.Setup(o => o.AssignCompleted(It.IsAny<bool>()))
                .Callback((bool result) =>
                {
                    creationResult = result;
                    createResetEvent.Set();
                });
            _productAssignStepMock.Setup(p => p.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Returns(Task.FromResult(productResult));
            _partsAssignStepMock.Setup(p => p.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Returns(Task.FromResult(partsResult));
            _recipeAssignStepMock.Setup(p => p.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Returns(Task.FromResult(recipeResult));
            _documentAssignStepMock.Setup(d => d.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Returns(Task.FromResult(documentResult));
            _validationStepMock.Setup(p => p.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Returns(Task.FromResult(validationResult));

            // Act
            _assignment.Assign(_operationDataMock.Object);
            createResetEvent.WaitOne(10000);

            //Assert
            Assert.That(creationResult, Is.EqualTo(expCreation));
        }

        [TestCase(false, false, false, false, false, false)]
        [TestCase(true, false, false, false, false, false)]
        [TestCase(true, true, false, false, false, false)]
        [TestCase(true, true, true, false, false, false)]
        [TestCase(true, true, true, true, false, false)]
        [TestCase(true, true, true, true, true, true)]
        [Description("If some component in the restore chain fails, the restore throws an exception")]
        public void RestoreFailureIfChainFails(bool productResult, bool partsResult, bool recipeResult, bool documentResult, bool userResult, bool expCreation)
        {
            // Arrange
            _productAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(productResult));
            _partsAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(partsResult));
            _recipeAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(recipeResult));
            _documentAssignStepMock.Setup(d => d.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(documentResult));
            _userAssignStepMock.Setup(d => d.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(userResult));

            // Act & Assert
            if (expCreation)
            {
                Assert.DoesNotThrowAsync(async delegate
                {
                    await _assignment.Restore(_operationDataMock.Object);
                    _productAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                    _partsAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                    _recipeAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                    _documentAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                    _userAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                });
            }
            else
            {
                Assert.ThrowsAsync<InvalidOperationException>(async () =>
                    await _assignment.Restore(_operationDataMock.Object));
            }
        }

        [Test(Description = "Restore should load assign the product again")]
        public void RestoreShouldLoadProductAndRecipe()
        {
            // Arrange
            _productAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(true));
            _partsAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(true));
            _recipeAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(true));
            _documentAssignStepMock.Setup(d => d.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(true));
            _userAssignStepMock.Setup(d => d.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(true));

            // Act
            Assert.DoesNotThrowAsync(async delegate
            {
                await _assignment.Restore(_operationDataMock.Object);
                _productAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                _partsAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                _recipeAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                _documentAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
                _userAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once);
            });
        }

        [Test(Description = "If the product assignment fails while restore, an exception will be thrown")]
        public void RestoreWithFailedProductAssignmentThrows()
        {
            // Arrange
            _productAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(false));
            _recipeAssignStepMock.Setup(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>())).Returns(Task.FromResult(false));

            // Act
            Assert.Throws<InvalidOperationException>(() => _assignment.Restore(_operationDataMock.Object).RunSynchronously());
            Assert.DoesNotThrow(() => _productAssignStepMock.Verify(p => p.RestoreStep(_operationDataMock.Object, It.IsAny<IOperationLogger>()), Times.Once));
        }

        [Test(Description = "Operation logger should hold log messages for an operation while creation")]
        public void OperationLoggerShouldHoldLogMessages()
        {
            // Arrange
            var createResetEvent = new ManualResetEvent(false);
            IOperationLogger logger = null;
            _productAssignStepMock.Setup(p => p.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Callback(delegate (IOperationData _, IOperationLogger operationLogger)
                {
                    logger = operationLogger;
                    operationLogger.Log(LogLevel.Information, "Hello pretty woman");
                    createResetEvent.Set();
                })
                .Returns(Task.FromResult(false));

            // Act
            _assignment.Assign(_operationDataMock.Object);
            createResetEvent.WaitOne(10000);

            // Assert
            Assert.That(logger, Is.Not.Null);
            Assert.That(logger.Messages.Count, Is.EqualTo(2));
        }
    }
}

