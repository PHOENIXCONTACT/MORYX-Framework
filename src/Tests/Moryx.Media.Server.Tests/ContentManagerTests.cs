// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Moryx.Media.Server.Previews;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging.Abstractions;
using Moryx.Logging;

namespace Moryx.Media.Server.Tests
{
    [TestFixture]
    public class ContentManagerTests
    {
        private ContentManager _contentManager;
        private string _testContentPath;
        private string _testStoragePath;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _testContentPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestContent");
            _testStoragePath = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Storage");
        }

        [SetUp]
        public void Setup()
        {
            var previewService = new Mock<IPreviewService>();

            _contentManager = new ContentManager
            {
                Config = new ModuleConfig { StoragePath = _testStoragePath },
                Logger = new ModuleLogger("Dummy", new NullLoggerFactory()),
                PreviewService = previewService.Object
            };

            _contentManager.Start();
        }

        [TearDown]
        public void TearDown()
        {
            _contentManager.Stop();

            Directory.Delete(_testStoragePath, true);
        }

        [TestCase("test.jpg", "image/jpeg")]
        [TestCase("test.png", "image/png")]
        [TestCase("test.txt", "text/plain")]
        public async Task AddContentAddsNewContent(string fileName, string mimeType)
        {
            // Arrange
            ContentAddingInfo addingInfo;

            byte[] masterContent;
            var testFileInfo = new FileInfo(Path.Combine(_testContentPath, fileName));

            // Act
            using (var stream = new FileStream(testFileInfo.FullName, FileMode.Open))
            {
                addingInfo = await _contentManager.AddMaster(fileName, stream);
            }

            //_contentManager.LoadDescriptors();
            var descriptor = _contentManager.GetDescriptor(addingInfo.Descriptor.Id);
            using (var master = _contentManager.GetStream(descriptor.GetMaster()))
            {
                masterContent = new byte[master.Length];
                master.Read(masterContent, 0, masterContent.Length);
            }

            // Assert
            var masterVariant = addingInfo.Descriptor.GetMaster();

            Assert.That(_contentManager.GetDescriptors(), Has.Count.EqualTo(1));
            Assert.That(addingInfo.Result, Is.EqualTo(ContentAddingResult.Ok));
            Assert.That("master", Is.EqualTo(masterVariant.Name));
            Assert.That("test", Is.EqualTo(descriptor.Name));
            Assert.That(string.IsNullOrEmpty(masterVariant.FileHash), Is.False);
            Assert.That(masterVariant.MimeType, Is.EqualTo(mimeType));
            Assert.That(testFileInfo, Has.Length.EqualTo(masterVariant.Size));
            Assert.That(masterVariant.Preview.Size, Is.EqualTo(0));
            Assert.That(Directory.EnumerateFiles(_testStoragePath, "*", SearchOption.AllDirectories).Count(), Is.EqualTo(3));
            Assert.That(SHA1.Create().ComputeHash(File.ReadAllBytes(testFileInfo.FullName)), Is.EqualTo(SHA1.Create().ComputeHash(masterContent)));
        }

        [Test]
        public async Task AddNewContentTwiceResultsInOneContent()
        {
            // Arrange
            ContentAddingInfo addingInfo = null;

            // Act
            for (int i = 0; i < 2; i++)
            {
                var path = Path.Combine(_testContentPath, "test.jpg");
                using (var contentStream = new FileStream(path, FileMode.Open))
                {
                    addingInfo = await _contentManager.AddMaster("test.jpg", contentStream);
                }
            }

            // Assert
            Assert.That(_contentManager.GetDescriptors(), Has.Count.EqualTo(1));
            Assert.That(addingInfo?.Result, Is.EqualTo(ContentAddingResult.AlreadyExists));
        }

        [Test]
        public async Task AddTwoFilesWithSameNameSucceeds()
        {
            // Arrange
            var files = new List<string>
            {
                "test.jpg",
                "test.png"
            };

            // Act
            foreach (var file in files)
            {
                var path = Path.Combine(_testContentPath, file);
                using (var contentStream = new FileStream(path, FileMode.Open))
                {
                    await _contentManager.AddMaster(file, contentStream);
                }
            }

            // Assert
            Assert.That(_contentManager.GetDescriptors(), Has.Count.EqualTo(2));
        }

        [TestCase(0u, 3u, 3u, null)]
        [TestCase(1u, 1u, 1u, null)]
        [TestCase(1u, 10u, 2u, null)]
        [TestCase(4u, 1u, 0u, typeof(ArgumentOutOfRangeException))]
        public async Task GetDescriptors(uint start, uint offset, uint expectedCount, Type exceptionType)
        {
            // Arrange
            var files = new List<string>
            {
                "test.jpg",
                "test.png",
                "test.txt"
            };

            // Act
            foreach (var file in files)
            {
                var path = Path.Combine(_testContentPath, file);
                using (var contentStream = new FileStream(path, FileMode.Open))
                {
                    await _contentManager.AddMaster(file, contentStream);
                }
            }

            IReadOnlyList<ContentDescriptor> descriptors = Array.Empty<ContentDescriptor>();

            try
            {
                descriptors = _contentManager.GetDescriptors(start, offset);
            }
            catch (Exception e)
            {
                Assert.That(e.GetType(), Is.EqualTo(exceptionType));
            }

            // Assert
            Assert.That(_contentManager.GetDescriptors().Count, Is.EqualTo(3));
            Assert.That(descriptors.Count, Is.EqualTo(expectedCount));
        }

        [Test]
        public void ResultOfNotExistentMasterIsNull()
        {
            // Arrange
            // Act
            var masterDescription = _contentManager.GetDescriptor(new Guid());

            // Assert
            Assert.That(masterDescription, Is.Null);
        }

        [Test]
        public async Task AddVariantSucceeds()
        {
            // Arrange
            ContentAddingInfo addingInfoVariant;
            byte[] masterContent;
            byte[] variantContent;

            const string masterFileName = "test.jpg";
            const string variantFileName = "test.png";
            const string variantName = "VariantTest";

            var testMasterFileInfo = new FileInfo(Path.Combine(_testContentPath, masterFileName));
            var testVariantFileInfo = new FileInfo(Path.Combine(_testContentPath, variantFileName));

            // Act
            var addingInfoMaster = await AddMaster(masterFileName);

            using (var variantStream = new FileStream(testVariantFileInfo.FullName, FileMode.Open))
            {
                addingInfoVariant = await _contentManager.AddVariant(addingInfoMaster.Descriptor.Id, variantName, variantFileName, variantStream);
            }

            var descriptor = _contentManager.GetDescriptor(addingInfoMaster.Descriptor.Id);
            var variantDescriptor = descriptor.GetVariant(variantName);

            using (var master = _contentManager.GetStream(descriptor.GetMaster()))
            {
                masterContent = new byte[master.Length];
                master.Read(masterContent, 0, masterContent.Length);
            }

            using (var variant = _contentManager.GetStream(variantDescriptor))
            {
                variantContent = new byte[variant.Length];
                variant.Read(variantContent, 0, variantContent.Length);
            }

            // Assert
            Assert.That(_contentManager.GetDescriptors().Count, Is.EqualTo(1));

            // Master
            var masterVariant = descriptor.GetMaster();

            Assert.That(addingInfoMaster.Result, Is.EqualTo(ContentAddingResult.Ok));
            Assert.That(masterVariant.Name, Is.EqualTo("master"));
            Assert.That(descriptor.Name, Is.EqualTo("test"));
            Assert.That(string.IsNullOrEmpty(masterVariant.FileHash), Is.False);
            Assert.That(masterVariant.MimeType, Is.EqualTo("image/jpeg"));
            Assert.That(masterVariant.Size, Is.EqualTo(testMasterFileInfo.Length));
            Assert.That(masterVariant.Preview.Size, Is.EqualTo(0));
            Assert.That(Directory.EnumerateFiles(_testStoragePath, "*", SearchOption.AllDirectories).Count(), Is.EqualTo(4));
            Assert.That(SHA1.Create().ComputeHash(masterContent), Is.EqualTo(SHA1.Create().ComputeHash(File.ReadAllBytes(testMasterFileInfo.FullName))));

            // Variant
            Assert.That(addingInfoVariant.Result, Is.EqualTo(ContentAddingResult.Ok));
            Assert.That(variantDescriptor.Name, Is.EqualTo(variantName));
            Assert.That(string.IsNullOrEmpty(variantDescriptor.FileHash), Is.False);
            Assert.That(variantDescriptor.MimeType, Is.EqualTo("image/png"));
            Assert.That(variantDescriptor.Size, Is.EqualTo(testVariantFileInfo.Length));
            Assert.That(variantDescriptor.Preview.Size, Is.EqualTo(0));
            Assert.That(SHA1.Create().ComputeHash(variantContent), Is.EqualTo(SHA1.Create().ComputeHash(File.ReadAllBytes(testVariantFileInfo.FullName))));
        }

        [Test]
        public async Task RemoveContent()
        {
            // Arrange
            var files = new List<string>
            {
                "test.jpg",
                "test.png"
            };

            foreach (var file in files)
            {
                var path = Path.Combine(_testContentPath, file);
                using (var contentStream = new FileStream(path, FileMode.Open))
                {
                    await _contentManager.AddMaster(file, contentStream);
                }
            }

            // Act
            var descriptor = _contentManager.GetDescriptors(0, 1).First();
            _contentManager.DeleteContent(descriptor.Id, "master");

            descriptor = _contentManager.GetDescriptors(0, 1).First();

            // Assert
            Assert.That(_contentManager.GetDescriptors().Count, Is.EqualTo(1));
            Assert.That(descriptor.GetMaster().Extension, Is.EqualTo(".png"));
            Assert.That(Directory.EnumerateFiles(_testStoragePath, "*", SearchOption.AllDirectories).Count(), Is.EqualTo(3));
        }

        private async Task<ContentAddingInfo> AddMaster(string fileName)
        {
            // Arrange
            ContentAddingInfo addingInfo;

            var testFileInfo = new FileInfo(Path.Combine(_testContentPath, fileName));

            // Act
            using (var stream = new FileStream(testFileInfo.FullName, FileMode.Open))
            {
                addingInfo = await _contentManager.AddMaster(fileName, stream);
            }

            return addingInfo;
        }
    }
}

