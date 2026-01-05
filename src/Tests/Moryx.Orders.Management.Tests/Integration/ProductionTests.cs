// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Recipes;
using Moryx.AbstractionLayer.TestTools;
using Moryx.ControlSystem.Jobs;
using Moryx.ControlSystem.TestTools;
using Moryx.StateMachines;
using Moryx.Threading;
using Moq;
using Moryx.Model.InMemory;
using Moryx.Model.Repositories;
using Moryx.Notifications;
using Moryx.Orders.Assignment;
using Moryx.Orders.Management.Assignment;
using Moryx.Orders.Management.Model;
using Moryx.Users;
using NUnit.Framework;
using Moryx.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class ProductionTests
    {
        private int _jobIdCounter;

        private IUnitOfWorkFactory<OrdersContext> _orderModel;
        private OperationDataPool _operationDataPool;
        private Mock<IJobManagement> _jobManagementMock;
        private Mock<IProductManagement> _productManagementMock;
        private ProductType _product;
        private IProductRecipe _recipe;
        private ProductIdentity _productIdentity;
        private User _user;

        [SetUp]
        public async Task SetUp()
        {
            _orderModel = new UnitOfWorkFactory<OrdersContext>(new InMemoryDbContextManager(Guid.NewGuid().ToString()));

            var logger = new ModuleLogger("Dummy", new NullLoggerFactory());
            var parallelOperations = new ParallelOperations(logger);

            var userMock = new Mock<User>();
            userMock.SetupGet(u => u.Identifier).Returns("1234");
            _user = userMock.Object;

            _operationDataPool = new OperationDataPool
            {
                UnitOfWorkFactory = _orderModel
            };

            _jobManagementMock = new Mock<IJobManagement>();
            _jobManagementMock.Setup(j => j.Evaluate(It.IsAny<ProductionRecipe>(), It.IsAny<int>()))
                .Returns(new JobEvaluation
                {
                    WorkplanErrors = new List<string>()
                });
            var jobManagement = _jobManagementMock.Object;

            var dispatcher = new SingleJobOperationDispatcher
            {
                JobManagement = jobManagement,
                ParallelOperations = parallelOperations
            };

            _productManagementMock = new Mock<IProductManagement>();
            var productManagement = _productManagementMock.Object;
            var jobHandler = new JobHandler
            {
                JobManagement = jobManagement,
                OperationDataPool = _operationDataPool,
                ParallelOperations = parallelOperations,
                Logger = logger,
                Dispatcher = dispatcher
            };

            var productAssignment = new DefaultProductAssignment
            {
                Logger = logger,
                ProductManagement = productManagement
            };

            var partsAssignement = new NullPartsAssignment();

            var recipeAssignment = new DefaultRecipeAssignment
            {
                Logger = logger,
                ProductManagement = productManagement,
            };

            var operationValidation = new NullOperationValidation();

            var productAssignStep = new ProductAssignStep
            {
                ProductAssignment = productAssignment,
                ProductManagement = productManagement
            };

            var partsAssignStep = new PartsAssignStep
            {
                PartsAssignment = partsAssignement
            };

            var recipeAssignStep = new RecipeAssignStep
            {
                RecipeAssignment = recipeAssignment,
                ProductManagement = productManagement,
                JobManagement = jobManagement,
                OperationDataPool = _operationDataPool
            };

            var userManagementMock = new Mock<IUserManagement>();
            var userAssignStep = new UserAssignStep
            {
                UserManagement = userManagementMock.Object
            };

            var validateAssignStep = new ValidateAssignStep
            {
                OperationValidation = operationValidation
            };

            var documentAssignStepMock = new Mock<IOperationAssignStep>();
            documentAssignStepMock.Setup(d => d.AssignStep(It.IsAny<IOperationData>(), It.IsAny<IOperationLogger>()))
                .Returns(Task.FromResult(true));

            var steps = new Dictionary<string, IOperationAssignStep>
            {
                {nameof(ProductAssignStep), productAssignStep},
                {nameof(PartsAssignStep), partsAssignStep},
                {nameof(RecipeAssignStep), recipeAssignStep},
                {nameof(UserAssignStep), userAssignStep},
                {nameof(ValidateAssignStep), validateAssignStep},
                {nameof(DocumentAssignStep), documentAssignStepMock.Object}
            };

            var stepFactory = new Mock<IAssignStepFactory>();
            stepFactory.Setup(f => f.Create(It.IsAny<string>())).Returns((string name) => steps[name]);

            var assignment = new OperationAssignment
            {
                AssignStepFactory = stepFactory.Object,
                LoggerProvider = new OperationLoggerProvider()
                {
                    Logger = logger
                }
            };

            var operationFactory = new OperationFactoryMock(logger, jobHandler, assignment);
            _operationDataPool.OperationFactory = operationFactory;

            await productAssignment.InitializeAsync(new ProductAssignmentConfig());
            await partsAssignement.InitializeAsync(new PartsAssignmentConfig());
            await recipeAssignment.InitializeAsync(new RecipeAssignmentConfig());
            await operationValidation.InitializeAsync(new OperationValidationConfig());

            await productAssignment.StartAsync();
            await partsAssignement.StartAsync();
            await recipeAssignment.StartAsync();
            await operationValidation.StartAsync();

            assignment.Start();

            await _operationDataPool.StartAsync();
            jobHandler.Start();

            // Prepare product management
            _productIdentity = new ProductIdentity("123456", 1);
            _product = new DummyProductType { Id = 1, Identity = _productIdentity };
            _recipe = new DummyRecipe { Id = 1 };

            _productManagementMock.Setup(p => p.LoadTypeAsync(_productIdentity)).ReturnsAsync(_product);
            _productManagementMock.Setup(p => p.LoadRecipesAsync(_product, RecipeClassification.Default)).ReturnsAsync([_recipe]);

            // Prepare jobs
            _jobManagementMock.Setup(j => j.AddAsync(It.IsAny<JobCreationContext>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((JobCreationContext creationContext, CancellationToken _) =>
                [
                    new Job(_recipe, (int)creationContext.Templates.Single().Amount)
                    {
                        Id = ++_jobIdCounter,
                        Classification = JobClassification.Waiting
                    }
                ]);
        }

        [Test(Description = "Runs a full production of a operation. At the end, a final report will be executed.")]
        //[Ignore("This test takes very long should be fixed!")]
        public async Task SimpleCompletedProduction()
        {
            const int amount = 10;

            // Create the operation
            // Arrange
            var readyEvent = new ManualResetEvent(false);
            var readyCallback = new EventHandler<OperationEventArgs>(delegate (object _, OperationEventArgs args)
            {
                if (args.OperationData.State.Classification == OperationStateClassification.Ready)
                    readyEvent.Set();
            });

            // Act
            _operationDataPool.OperationUpdated += readyCallback;
            await _operationDataPool.Add(CreateOperationContext(_product), new NullOperationSource());
            var readyReachedResult = readyEvent.WaitOne(60000);
            _operationDataPool.OperationUpdated -= readyCallback;

            var operationData = _operationDataPool.GetAll().Single();
            var operation = operationData.Operation;

            // Assert
            // Operation should be created and ready now
            Assert.That(readyReachedResult, "Operation does not get ready for production");
            Assert.That(operationData.State.CanBegin);

            // Act
            // We begin the operation with the full amount
            await operationData.Adjust(amount, _user);

            // Assert
            // A job should be dispatched
            Assert.DoesNotThrow(() => _jobManagementMock.Verify(j => j.AddAsync(It.IsAny<JobCreationContext>()), Times.Once));
            Assert.That(operation.Jobs.Count, Is.EqualTo(1));

            var job = operation.Jobs.Single();

            // Lets produce all the requested Amount
            // Arrange
            var amountReachedEvent = new ManualResetEvent(false);
            var amountReachedCallback = new EventHandler<OperationEventArgs>(delegate
            {
                amountReachedEvent.Set();
            });

            // Act
            _operationDataPool.OperationUpdated += amountReachedCallback;
            SimulateProduction(job);
            var amountReachedResult = amountReachedEvent.WaitOne(60000);
            _operationDataPool.OperationUpdated -= amountReachedCallback;

            // Assert
            // Amount should be reached now
            Assert.That(amountReachedResult, "Simulating production failed");
            var beginContext = operationData.GetBeginContext();
            Assert.That(beginContext.PartialAmount, Is.EqualTo(amount));

            // Lets do a final report
            // Arrange
            var completedRaised = false;
            var completedCallback = new EventHandler<OperationEventArgs>((_, _) => completedRaised = true);

            // Act
            _operationDataPool.OperationUpdated += completedCallback;
            await operationData.Report(new OperationReport(ConfirmationType.Final, amount, 0, _user));
            _operationDataPool.OperationUpdated -= completedCallback;

            // Assert
            Assert.That(completedRaised);
            Assert.That(operationData.Operation.ReportedSuccessCount(), Is.EqualTo(amount));
            Assert.That(operationData.State.CanBegin, Is.False);
            Assert.That(operationData.State.CanInterrupt, Is.False);
            Assert.That(operationData.State.CanPartialReport, Is.False);
            Assert.That(operationData.State.CanFinalReport, Is.False);
            Assert.That(operationData.State.CanAdvice);

            // Do a final database comparison
            AssertOperationEntity(operationData);
        }

        private void SimulateProduction(Job job)
        {
            job.Classification = JobClassification.Running;
            RaiseJobUpdated(job);

            for (var i = 1; i < job.Amount; i++)
            {
                job.SuccessCount = i;
                RaiseJobUpdated(job);
            }

            job.SuccessCount = job.Amount;
            job.Classification = JobClassification.Completed;
            RaiseJobUpdated(job);
        }

        private void RaiseJobUpdated(Job job)
        {
            _jobManagementMock.Raise(j => j.StateChanged += null, _jobManagementMock.Object, new JobStateChangedEventArgs(job, JobClassification.Idle, job.Classification));
        }

        private static OperationCreationContext CreateOperationContext(ProductType product)
        {
            var productIdentity = (ProductIdentity)product.Identity;

            var orderContext = new OrderCreationContext
            {
                Number = "300203003333"
            };

            var operationContext = new OperationCreationContext
            {
                TotalAmount = 10,
                Name = "IntegrationTests",
                Number = "0010",
                Order = orderContext,
                OverDeliveryAmount = 11,
                UnderDeliveryAmount = 9,
                ProductIdentifier = productIdentity.Identifier,
                ProductRevision = productIdentity.Revision,
                Parts = Array.Empty<PartCreationContext>()
            };
            orderContext.Operations.Add(operationContext);

            return operationContext;
        }

        private void AssertOperationEntity(IOperationData operationData)
        {
            var operation = operationData.Operation;
            using var uow = _orderModel.Create();

            var operationRepo = uow.GetRepository<IOperationEntityRepository>();
            var operationEntity = operationRepo.Linq.FirstOrDefault(o => o.Identifier.Equals(operationData.Identifier));

            Assert.That(operationEntity, Is.Not.Null);
            Assert.That(operationData.Number, Is.EqualTo(operationEntity.Number));
            Assert.That(operation.Name, Is.EqualTo(operationEntity.Name));
            Assert.That(operation.OverDeliveryAmount, Is.EqualTo(operationEntity.OverDeliveryAmount));
            Assert.That(operation.UnderDeliveryAmount, Is.EqualTo(operationEntity.UnderDeliveryAmount));
            Assert.That(((StateBase)operationData.State).Key, Is.EqualTo(operationEntity.State));
            Assert.That(operationData.TargetAmount, Is.EqualTo(operationEntity.TargetAmount));
            Assert.That(operationData.TotalAmount, Is.EqualTo(operationEntity.TotalAmount));
            Assert.That(operationData.OrderData.Number, Is.EqualTo(operationEntity.Order.Number));
        }
    }
}

