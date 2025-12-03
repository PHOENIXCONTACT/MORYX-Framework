// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moryx.ControlSystem.Jobs;
using Moryx.Logging;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Orders.Management.Assignment;
using Moryx.Orders.Management.Model;
using NUnit.Framework;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class OperationPoolTests
    {
        private IUnitOfWorkFactory<OrdersContext> _unitOfWorkFactory;
        private OperationDataPool _operationDataPool;
        private IModuleLogger _moduleLogger = new ModuleLogger("Dummy", new NullLoggerFactory());
        private Mock<IOperationData> _operationDataMock;
        private Mock<IOperationFactory> _operationFactoryMock;
        private Mock<IOperationAssignment> _operationAssignmentMock;
        private Mock<IJobHandler> _jobHandlerMock;
        private Mock<ICountStrategy> _countStrategyMock;

        [SetUp]
        public void Setup()
        {
            _unitOfWorkFactory = new UnitOfWorkFactory<OrdersContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));

            _operationDataMock = SetupOperationDataMock();

            _operationFactoryMock = new Mock<IOperationFactory>();
            _operationFactoryMock.Setup(opf => opf.Create()).Returns(() => _operationDataMock.Object);

            _operationDataPool = new OperationDataPool
            {
                UnitOfWorkFactory = _unitOfWorkFactory,
                OperationFactory = _operationFactoryMock.Object,
                Logger = _moduleLogger
            };

            _operationAssignmentMock = new Mock<IOperationAssignment>();
            _jobHandlerMock = new Mock<IJobHandler>();
            _jobHandlerMock.Setup(h => h.Restore(It.Is<IEnumerable<long>>(e => !e.Any()))).Returns(Array.Empty<Job>());
            _countStrategyMock = new Mock<ICountStrategy>();
        }

        private Mock<IOperationData> SetupOperationDataMock()
        {
            var mock = new Mock<IOperationData>();
            mock.SetupAllProperties();
            mock.Setup(opd => opd.Initialize(It.IsAny<OperationCreationContext>(), It.IsAny<OrderData>(), It.IsAny<IOperationSource>()))
                .Returns<OperationCreationContext, OrderData, IOperationSource>((context, _, _) =>
                {

                    mock.SetupGet(od => od.Number).Returns(context.Number);
                    return mock.Object;
                });
            mock.Setup(opd => opd.Initialize(It.IsAny<OperationEntity>(), It.IsAny<OrderData>()))
                .Returns(() => mock.Object);
            return mock;
        }

        [Test(Description = "The start of the OperationPool should restore an OperationData from the database")]
        public async Task StartTheOperationPool()
        {
            // Arrange
            using var uow = _unitOfWorkFactory.Create();
            var orderEntity = uow.GetRepository<IOrderEntityRepository>().Create("314725836");
            var operationEntity = uow.GetRepository<IOperationEntityRepository>().Create();
            operationEntity.Order = orderEntity;
            operationEntity.Number = "0010";
            operationEntity.ProductId = 1;
            await uow.SaveChangesAsync();

            //Act
            await _operationDataPool.StartAsync();

            //Assert
            _operationDataMock.Verify(opd => opd.Initialize(
                It.IsAny<OperationEntity>(),
                It.IsAny<OrderData>()),
                Times.Once, "The OperationData should be initialized after the start of the OperationPool");
            _operationDataMock.Verify(opd => opd.Restore(),
                Times.Once, "The OperationData should be restored after the start of the OperationPool");
            Assert.That(_operationDataPool.GetAll().Count, Is.EqualTo(1), "There should be one Operation after the start of the Pool");
            Assert.That(_operationDataPool.GetOrders().Count, Is.EqualTo(1), "There should be one Operation after the start of the Pool");
        }

        [Test(Description = "The OperationPool should only load not completed operations from the database during the start")]
        public async Task RestoreOnlyNotCompletedOperations()
        {
            // Arrange
            using var uow = _unitOfWorkFactory.Create();
            var orderEntity = uow.GetRepository<IOrderEntityRepository>().Create("12345678");

            var orderData = OperationStorage.LoadOrder(orderEntity);
            _operationDataMock.SetupGet(op => op.OrderData).Returns(() => orderData);

            var operationEntity = uow.GetRepository<IOperationEntityRepository>()
                .Create(Guid.NewGuid(), "0010", 0, DateTime.Now, DateTime.Now);
            operationEntity.Order = orderEntity;
            operationEntity.ProductId = 1;
            await uow.SaveChangesAsync();

            operationEntity = uow.GetRepository<IOperationEntityRepository>().Create(Guid.NewGuid(), "0020",
                OperationDataStateBase.CompletedKey, DateTime.Now, DateTime.Now);
            operationEntity.Order = orderEntity;
            operationEntity.ProductId = 1;
            await uow.SaveChangesAsync();

            // Act
            await _operationDataPool.StartAsync();

            // Assert
            _operationDataMock.Verify(opd => opd.Initialize(
                It.IsAny<OperationEntity>(),
                It.IsAny<OrderData>()),
                Times.Once, "The OperationData should be initialized after the start of the OperationPool");
            _operationDataMock.Verify(opd => opd.Restore(),
                Times.Once, "The OperationData should be restored after the start of the OperationPool");
            _operationDataMock.Verify(opd => opd.Resume(),
                Times.Once, "The OperationData should be resumed after the startof the OperationPool");
            Assert.That(_operationDataPool.GetAll().Count, Is.EqualTo(1), "There should be only one Operation after the start of the Pool");
            Assert.That(_operationDataPool.GetOrders().Count, Is.EqualTo(1), "There should be only one Operation after the start of the Pool");
        }

        [Test(Description = "The OperationData should have all ProductParts and Advices after the restoring during the start of the OperationPool")]
        public async Task ShouldRestoreProductPartsAndAdvices()
        {
            // Arrange
            using var uow = _unitOfWorkFactory.Create();
            var orderEntity = uow.GetRepository<IOrderEntityRepository>().Create("12345678");
            var productPartRepo = uow.GetRepository<IProductPartEntityRepository>();
            var productPart1 = productPartRepo.Create("Test1", "1234", 4, "PinÃ¶ckel");
            var productPart2 = productPartRepo.Create("Test2", "5678", 2, "Tucken");
            var adviceRepo = uow.GetRepository<IOperationAdviceEntityRepository>();
            var advice1 = adviceRepo.Create("BrottÃ¼te", 4);
            var advice2 = adviceRepo.Create("Schnodderlappen", 2);
            var operationEntity = uow.GetRepository<IOperationEntityRepository>()
                .Create(Guid.NewGuid(), "1234", 30, DateTime.Now, DateTime.Now);
            operationEntity.Order = orderEntity;
            operationEntity.ProductId = 1;
            operationEntity.ProductParts.Add(productPart1);
            operationEntity.ProductParts.Add(productPart2);
            operationEntity.Advices.Add(advice1);
            operationEntity.Advices.Add(advice2);
            operationEntity.Source = "{ \"$type\":\"Moryx.Orders.NullOperationSource, Moryx.Orders\" }";
            await uow.SaveChangesAsync();

            var operationData = new OperationData
            {
                OperationAssignment = _operationAssignmentMock.Object,
                JobHandler = _jobHandlerMock.Object,
                Logger = _moduleLogger,
                CountStrategy = _countStrategyMock.Object,
            };

            _operationFactoryMock.Setup(opf => opf.Create()).Returns(() => operationData);

            // Act
            await _operationDataPool.StartAsync();

            // Assert
            Assert.That(operationData.Operation.Parts.Count, Is.EqualTo(2), "There should be 2 restored product parts");
            Assert.That(operationData.Operation.Advices.Count, Is.EqualTo(2), "There should be 2 restored advices");
        }

        [Test(Description = "Add an operation to the pool and check the returned object")]
        public async Task AddOperationWithNewOrder()
        {
            //Arrange
            var operationContext = CreateOperationContext();
            var operationSource = new NullOperationSource();

            //Act
            await _operationDataPool.Add(operationContext, operationSource);

            //Assert
            Assert.That(_operationDataPool.GetAll().Count, Is.EqualTo(1), "One message was added to the pool, but did not come back");
            _operationDataMock.Verify(o => o.Initialize(operationContext, It.IsAny<OrderData>(), operationSource),
                Times.Once, "The OperationData should be initialized after adding it to the OperationPool");
        }

        [Test(Description = "If a new operation should be added to an existing order then it should be loaded from the database")]
        public async Task AddOperationToAnExistingOrder()
        {
            // Arrange
            using var uow = _unitOfWorkFactory.Create();

            var orderRepo = uow.GetRepository<IOrderEntityRepository>();
            var orderEntity = orderRepo.Create("12345678");

            var orderData = OperationStorage.LoadOrder(orderEntity);
            _operationDataMock.SetupGet(op => op.OrderData).Returns(() => orderData);

            // Add a completed operation to the database
            var operationEntity = await uow.GetRepository<IOperationEntityRepository>().CreateAsync();
            operationEntity.Order = orderEntity;
            operationEntity.Number = "0010";
            operationEntity.ProductId = 1;
            operationEntity.State = OperationDataStateBase.CompletedKey;
            await uow.SaveChangesAsync();

            var operationContext = CreateOperationContext();

            // Act
            await _operationDataPool.StartAsync();
            await _operationDataPool.Add(operationContext, new NullOperationSource());

            // Assert
            Assert.That(_operationDataPool.GetAll().Count, Is.EqualTo(1), "There should be only one operation after the start");

            var orderEntities = await orderRepo.GetAllAsync();
            Assert.That(orderEntities.Count, Is.EqualTo(1), "There should only one order in the database");
        }

        [TestCase("123456", "1234", false, Description = "Get operation from the pool which is in the current operation list")]
        [TestCase("", "", true, Description = "The OperationPool returns a null for the requested order and operation which are unknown")]
        public async Task GetOperationFromCurrentOperationList(string orderNumber, string operationNumber, bool resultShouldBeNull)
        {
            // Arrange
            var operationContext = CreateOperationContext("123456", "1234");
            await _operationDataPool.StartAsync();
            await _operationDataPool.Add(operationContext, new NullOperationSource());
            _operationDataMock.SetupGet(op => op.Number).Returns("1234");

            var orderData = new OrderData();
            orderData.Order.Number = "123456";
            _operationDataMock.SetupGet(op => op.OrderData).Returns(orderData);

            // Act
            var operation = await _operationDataPool.Get(orderNumber, operationNumber);

            // Assert
            if (resultShouldBeNull)
            {
                Assert.That(operation, Is.Null, "There should be no operation for the given data");
            }
            else
            {
                Assert.That(operation, Is.Not.Null, "There should be an operation with the given order and operation number");
            }
        }

        [Test(Description = "Should not throw an exception for new identical operation number/order number after completing previous one.")]
        public async Task ShouldNotThrowAnExceptionForSecondOperation()
        {
            //Step 1
            // Arrange
            const string operationNumber = "123456";
            const string orderNumber = "1234";
            var operationDataMock1 = await InitializeOperation(operationNumber, orderNumber, _operationDataPool, true);

            // Act
            var operation1 = await _operationDataPool.Get(orderNumber, operationNumber);
            operationDataMock1.SetupGet(op => op.State)
                .Returns(new CompletedState(null, null));
            //Assert
            Assert.That(operation1.State, Is.TypeOf<CompletedState>());

            // step 2
            //Arrange
            var operationDataMock2 = InitializeOperation(operationNumber, orderNumber, _operationDataPool, false);

            IOperationData notCompletedOperation = null;
            Assert.DoesNotThrowAsync(async () => notCompletedOperation = await _operationDataPool.Get(orderNumber, operationNumber));
            //Assert
            Assert.That(notCompletedOperation, Is.Not.Null, "There should be an operation with the given order and operation number");
        }

        private async Task<Mock<IOperationData>> InitializeOperation(string operationNumber, string orderNumber,
            OperationDataPool operationDataPool, bool startOperationPool)
        {
            var context = CreateOperationContext(orderNumber, operationNumber);
            var mock = new Mock<IOperationData>();
            mock = SetupOperationDataMock();
            _operationFactoryMock.Setup(opf => opf.Create()).Returns(mock.Object);

            if (startOperationPool)
                await operationDataPool.StartAsync();

            await operationDataPool.Add(context, new NullOperationSource());
            mock.SetupGet(op => op.State).Returns(new ReadyState(null, null));
            var orderData = new OrderData();
            orderData.Order.Number = orderNumber;
            mock.SetupGet(op => op.OrderData).Returns(orderData);
            mock.SetupGet(op => op.Number).Returns(operationNumber);
            return mock;
        }

        [Test(Description = "Get operation from the database is it is already completed")]
        public async Task GetOperationFromDatabase()
        {
            // Arrange
            const string orderNumber = "12345678";
            const string operationNumber = "0020";

            using (var uow = _unitOfWorkFactory.Create())
            {
                var orderEntity = uow.GetRepository<IOrderEntityRepository>().Create(orderNumber);

                var orderData = OperationStorage.LoadOrder(orderEntity);
                _operationDataMock.SetupGet(op => op.OrderData).Returns(() => orderData);

                var operationEntity = await uow.GetRepository<IOperationEntityRepository>().CreateAsync();
                operationEntity.Order = orderEntity;
                operationEntity.Number = operationNumber;
                operationEntity.ProductId = 1;
                operationEntity.State = OperationDataStateBase.CompletedKey;
                await uow.SaveChangesAsync();
            }

            await _operationDataPool.StartAsync();

            // Act
            var operation = _operationDataPool.Get(orderNumber, operationNumber);

            // Assert
            Assert.That(operation, Is.Not.Null, "There should be an operation loaded from the database");

        }

        private static OperationCreationContext CreateOperationContext(string orderNumber = "12345678", string operationNumber = "zwÃ¶lf")
        {
            var orderContext = new OrderCreationContext
            {
                Number = orderNumber,
                MaterialParameters = { new DummyMaterialParameter("FooKey", "FooValue") }
            };

            var operationContext = new OperationCreationContext
            {
                TotalAmount = 10,
                Name = "elf",
                Number = operationNumber,
                Order = orderContext,
                OverDeliveryAmount = 13,
                UnderDeliveryAmount = 7,
                ProductIdentifier = "vierzehn",
                ProductRevision = 15
            };

            orderContext.Operations.Add(operationContext);

            return operationContext;
        }
    }

    public class DummyMaterialParameter : IMaterialParameter
    {
        public DummyMaterialParameter(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }

        public string Value { get; }
    }
}

