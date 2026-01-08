// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.TestTools;
using Moryx.Logging;
using Moq;
using Moryx.Orders.Assignment;
using Moryx.Orders.Documents;
using Moryx.Orders.Management.Assignment;
using NUnit.Framework;
using System.Runtime.InteropServices;
using System.Threading;

namespace Moryx.Orders.Management.Tests
{
    [TestFixture]
    public class DocumentAssignStepTests
    {
        private DocumentAssignStep _documentAssignStep;
        private string _savePath;

        private IOperationData _operationData;
        private Mock<IDocumentLoader> _documentLoaderMock;
        private string _materialDataFolder;
        private Mock<IModuleLogger> _loggerMock;
        private ProductIdentity _productIdentity;
        private InternalOperation _operation;

        [OneTimeSetUp]
        public void TestSetup()
        {
            _savePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "OperationAssignmet");
            _productIdentity = new ProductIdentity("4711", 0);
            _materialDataFolder = Path.Combine(_savePath, _productIdentity.ToString());
        }

        [SetUp]
        public void Setup()
        {
            if (Directory.Exists(_materialDataFolder))
                Directory.Delete(_materialDataFolder, true);

            _documentLoaderMock = new Mock<IDocumentLoader>();
            var documentProviderConfig = new DocumentsConfig
            {
                DocumentLoader = new DocumentLoaderConfig { PluginName = "Mock" },
                DocumentsPath = _savePath
            };

            _loggerMock = new Mock<IModuleLogger>();
            _loggerMock.Setup(l => l.GetChild(It.IsAny<string>(), It.IsAny<Type>())).Returns(_loggerMock.Object);
            _documentAssignStep = new DocumentAssignStep
            {
                DocumentLoader = _documentLoaderMock.Object,
                Logger = _loggerMock.Object,
                ModuleConfig = new ModuleConfig
                {
                    Documents = documentProviderConfig
                }
            };

            _documentAssignStep.Start();

            var productType = new DummyProductType { Identity = _productIdentity };

            _operation = new InternalOperation
            {
                Number = "1001",
                Product = productType
            };

            var orderMock = new Mock<IOrderData>();
            orderMock.SetupGet(o => o.Number).Returns("1001");

            var operationDataMock = new Mock<IOperationData>();
            operationDataMock.SetupGet(o => o.Operation).Returns(_operation);
            operationDataMock.SetupGet(o => o.OrderData).Returns(orderMock.Object);
            operationDataMock.SetupGet(o => o.Product).Returns(_operation.Product);
            operationDataMock.SetupGet(o => o.Number).Returns(_operation.Number);

            _operationData = operationDataMock.Object;
        }

        [TearDown]
        public void TearDown()
        {
            _documentAssignStep = null;
        }

        [Test(Description = "Assigns documents which are loaded by the document loader and saves them locally into the data folder")]
        public async Task AssignDocuments()
        {
            // Arrange
            var operationLoggerMock = new Mock<IOperationLogger>();
            _documentLoaderMock.Setup(l => l.LoadAsync(_operation, It.IsAny<CancellationToken>())).ReturnsAsync(
            [
                new FileSystemDocument("29025550", 0, Path.Combine(_savePath, "29025550-00.txt"))
                {
                    ContentType = "text/plain",
                    Description = "",
                    Type = "",
                }
            ]);

            // Act
            await _documentAssignStep.AssignStep(_operationData, operationLoggerMock.Object);

            // Assert
            var files = Directory.GetFiles(_materialDataFolder).Select(Path.GetFileName).ToArray();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.That(files, Does.Contain("29025550-00.txt"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Assert.That(files, Does.Contain("29025550-00"));
            }
            else
            {
                Assert.Fail("OS Platform not supported");
            }
            Assert.That(files, Does.Contain("29025550-00.json"));
            Assert.That(_operation.Documents.Count, Is.EqualTo(1));
        }

        [Test(Description = "Uses the data folder to restore documents from json")]
        public async Task RestoreDocuments()
        {
            // Arrange
            await AssignDocuments();
            var operationLoggerMock = new Mock<IOperationLogger>();
            _operation.Documents = Array.Empty<Document>();

            // Act
            await _documentAssignStep.RestoreStep(_operationData, operationLoggerMock.Object);

            // Assert
            Assert.That(_operation.Documents.Count, Is.EqualTo(1));
        }
    }
}
