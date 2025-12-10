// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Threading.Tasks;
using Moq;
using Moryx.Orders.Advice;
using Moryx.Orders.Management.Advice;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class AdviceManagerTests
    {
        private AdviceManager _adviceManager;
        private Mock<IAdviceExecutor> _adviceExecutorMock;
        private ModuleConfig _config;
        private Mock<IOperationData> _operationDataMock;
        private InternalOperation _operation;

        [SetUp]
        public Task SetUp()
        {
            _adviceExecutorMock = new Mock<IAdviceExecutor>();
            _config = new ModuleConfig();

            _operation = new InternalOperation();

            _operationDataMock = new Mock<IOperationData>();
            _operationDataMock.SetupGet(o => o.Operation).Returns(_operation);

            _adviceManager = new AdviceManager
            {
                AdviceExecutor = _adviceExecutorMock.Object,
                ModuleConfig = _config
            };

            _adviceExecutorMock.Setup(e => e.Advice(_operation, It.IsAny<OrderAdvice>()))
                .ReturnsAsync((Operation _, OrderAdvice advice) => new AdviceResult(advice));

            _adviceExecutorMock.Setup(e => e.Advice(_operation, It.IsAny<PickPartAdvice>()))
                .ReturnsAsync((Operation _, PickPartAdvice advice) => new AdviceResult(advice));

            return _adviceManager.StartAsync();
        }

        [Test(Description = "Executor should handle order advices")]
        public async Task OrderAdviceShouldBeHandledByTheExecutor()
        {
            // Arrange
            _config.Advice.UseAdviceExecutorForOrderAdvice = true;

            // Act
            await _adviceManager.OrderAdvice(_operationDataMock.Object, "123456789", 10);

            // Assert
            _adviceExecutorMock.Verify(e => e.Advice(_operation, It.IsAny<OrderAdvice>()),
                Times.Once, "There should be an OrderAdvice handled by the Executor");
            _operationDataMock.Verify(o => o.Advice(It.IsAny<OrderAdvice>()), Times.Once,
                "There should be an order advice at the OperationData");
        }

        [TestCase(true, Description = "The pick part advice should be handled by the executor")]
        [TestCase(false, Description = "The pick part advice should be handled by the executor")]
        public void PickPartAdvicesShouldBeHandledTheExecutor(bool executorConfig)
        {
            // Arrange
            _config.Advice.UseAdviceExecutorForOrderAdvice = executorConfig;
            var part = new ProductPart
            {
                StagingIndicator = StagingIndicator.PickPart
            };

            // Act
            _adviceManager.PickPartAdvice(_operationDataMock.Object, "123456789", part);

            // Assert
            _adviceExecutorMock.Verify(e => e.Advice(_operation, It.IsAny<PickPartAdvice>()),
                Times.Once, "There should be a PickPartAdvice handled by the Executor");
            _operationDataMock.Verify(o => o.Advice(It.IsAny<PickPartAdvice>()), Times.Once,
                "There should be a pick part advice at the OperationData");
        }

        [Test(Description = "The executor should not be used for order advices if it disabled for order advices")]
        public void ExecutorShouldNotHandleOrderAdvice()
        {
            // Arrange
            _config.Advice.UseAdviceExecutorForOrderAdvice = false;

            // Act
            _adviceManager.OrderAdvice(_operationDataMock.Object, "123456789", 10);

            // Assert
            _adviceExecutorMock.Verify(e => e.Advice(_operation, It.IsAny<OrderAdvice>()),
                Times.Never, "There should be no OrderAdvice handled by the Executor");
            _operationDataMock.Verify(o => o.Advice(It.IsAny<OrderAdvice>()), Times.Once,
                "There should be an order advice at the OperationData");
        }
    }
}

