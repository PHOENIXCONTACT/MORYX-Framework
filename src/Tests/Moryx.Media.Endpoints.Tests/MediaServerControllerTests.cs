// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Moryx.Media.Endpoints.Tests
{
    public class MediaServerControllerTests
    {
        private MediaServerController _controller;
        private Mock<IMediaServer> _mediaServerMock;
        private string _testContentPath;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _testContentPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestContent");
        }

        [SetUp]
        public void Setup()
        {
            _mediaServerMock = new Mock<IMediaServer>();
            _controller = new MediaServerController(_mediaServerMock.Object);

            _mediaServerMock.SetupFileSize(10);
            _mediaServerMock.SetupSupportedTypes(".png", ".jpeg", ".jpg", ".gif", ".pdf", ".txt", ".csv");
        }

        [TestCase("11111111-1111-1111-1111-111111111111", "11111111-1111-1111-1111-111111111111", typeof(OkResult), Description = "Content successfully retrieved")]
        [TestCase("11111111-1111-1111-1111-111111111111", "00000000-0000-0000-0000-000000000000", typeof(NotFoundObjectResult), Description = "Content not found")]
        [TestCase("11111111-1111-1111-1111-XXXXXXXXXXXX", "00000000-0000-0000-0000-000000000000", typeof(BadRequestResult), Description = "Invalid guid caused bad request result.")]
        public void ShouldGetContentById(string requestedGuid, string availableGuid, Type resultType)
        {
            // Arrange
            var parsedAvailableGuid = Guid.Parse(availableGuid);
            var returnedContent = new ContentDescriptor(parsedAvailableGuid);
            _mediaServerMock.Setup(ms => ms.Get(It.Is<Guid>(g => g == parsedAvailableGuid))).Returns(returnedContent);

            // Act
            var actionResult = _controller.Get(requestedGuid);
            // Assert
            if (actionResult.Result is null)
            {
                Assert.That(actionResult.Value.Id, Is.EqualTo(returnedContent.Id));
                Assert.That(actionResult.Value.Name, Is.EqualTo(returnedContent.Name));
                Assert.That(actionResult.Value.Variants.Length, Is.EqualTo(returnedContent.Variants.Count));
            }

            else
                Assert.That(actionResult.Result.GetType().IsAssignableTo(resultType));
        }

        [TestCase("11111111-1111-1111-1111-111111111111", "11111111-1111-1111-1111-111111111111", typeof(OkResult), Description = "Variant successfully retrieved")]
        [TestCase("11111111-1111-1111-1111-111111111111", "00000000-0000-0000-0000-000000000000", typeof(NotFoundObjectResult), Description = "Variant not found")]
        [TestCase("11111111-1111-1111-1111-XXXXXXXXXXXX", "00000000-0000-0000-0000-000000000000", typeof(BadRequestResult), Description = "Invalid guid caused bad request result.")]
        public void ShouldGetVariantByIdAndName(string requestedGuid, string availableGuid, Type resultType)
        {
            // Arrange
            var parsedAvailableGuid = Guid.Parse(availableGuid);
            var returnedVariant = new VariantDescriptor();
            _mediaServerMock.Setup(ms => ms.GetVariant(It.Is<Guid>(g => g == parsedAvailableGuid), It.IsAny<string>()))
                .Returns(returnedVariant);

            // Act
            var actionResult = _controller.GetVariant(requestedGuid, "TestVariantName");

            // Assert
            if (actionResult.Result is null)
                Assert.That(actionResult.Value, Is.EqualTo(returnedVariant));
            else
                Assert.That(actionResult.Result.GetType().IsAssignableTo(resultType));
        }

        [TestCase("SomeFileHash", "image/jpeg", typeof(FileStreamResult), Description = "Successfully recieve FileStream")]
        [TestCase("SomeFileHash", "WrongMimeType", typeof(NotFoundObjectResult), Description = "No matching file found")]
        [TestCase(null, null, typeof(BadRequestResult), Description = "Invalid FileDescriptor caused bad request result.")]
        public void ShouldGetStreamFromFileDescriptor(string fileHash, string mimeType, Type resultType)
        {
            // Arrange
            var guid = Guid.NewGuid();
            var fileDescriptor = new VariantDescriptor() { Name = "name", FileHash = fileHash, MimeType = mimeType };
            var returnedStream = Stream.Null;
            _mediaServerMock.Setup(ms => ms.GetStream(It.Is<FileDescriptor>(g => g.MimeType == "image/jpeg")))
                .Returns(returnedStream);
            _mediaServerMock.Setup(ms => ms.GetVariant(It.IsAny<Guid>(), "name")).Returns(fileDescriptor);
            // Act
            var actionResult = _controller.GetVariantStream(guid.ToString(), fileDescriptor.Name, false);

            // Assert
            Assert.That(actionResult.Result.GetType().IsAssignableTo(resultType));
        }

        [TestCase("test.jpg", "image/jpeg")]
        [TestCase("test.jpeg", "image/jpeg")]
        [TestCase("test.png", "image/png")]
        [TestCase("test.gif", "image/gif")]
        [TestCase("test.txt", "text/plain")]
        [TestCase("test.pdf", "application/pdf")]
        public async Task ShouldAcceptUploadOfTestContentAsync(string fileName, string mimeType)
        {
            // Arrange
            var targetGuid = Guid.NewGuid();
            var contentAddingInfo = new ContentAddingInfo { Descriptor = new ContentDescriptor(targetGuid) };
            _mediaServerMock.Setup(ms => ms.AddMasterAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(contentAddingInfo));

            using var stream = File.OpenRead(Path.Combine(_testContentPath, fileName));
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = mimeType
            };

            // Act
            var actionResult = await _controller.AddMaster(file);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(targetGuid));
        }

        [TestCase("test.jpg", "image/jpg")]
        public async Task ShouldNotAcceptUploadOfInvalidFileSignatureAsync(string fileName, string mimeType)
        {
            // Arrange
            var targetGuid = Guid.NewGuid();
            var contentAddingInfo = new ContentAddingInfo { Descriptor = new ContentDescriptor(targetGuid) };
            _mediaServerMock.Setup(ms => ms.AddMasterAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(contentAddingInfo));

            // smuggling a txt file as a jpg should not work
            using var stream = File.OpenRead(Path.Combine(_testContentPath, "test.txt"));
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name).Replace(".txt", ".jpg"))
            {
                Headers = new HeaderDictionary(),
                ContentType = mimeType
            };

            // Act
            var actionResult = await _controller.AddMaster(file);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Does.Not.Contain("file type isn't permitted"));
            Assert.That(result.Value, Does.Contain("signature doesn't match"));
        }

        [TestCase("test.txt", "plain/txt")]
        public async Task ShouldNotAcceptUploadOfForbiddenFileTypeAsync(string filename, string mimeType)
        {
            // Arrange
            var targetGuid = Guid.NewGuid();
            var contentAddingInfo = new ContentAddingInfo { Descriptor = new ContentDescriptor(targetGuid) };
            _mediaServerMock.Setup(ms => ms.AddMasterAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(contentAddingInfo));
            _mediaServerMock
                .Setup(ms => ms.GetSupportedFileTypes())
                .Returns([".png", ".jpeg"]);

            using var stream = File.OpenRead(Path.Combine(_testContentPath, filename));
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = mimeType
            };

            // Act
            var actionResult = await _controller.AddMaster(file);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.That(result.StatusCode, Is.EqualTo(400));
            Assert.That(result.Value, Does.Contain("file type isn't permitted"));
            Assert.That(result.Value, Does.Not.Contain("signature doesn't match"));
        }

        public async Task ShouldAcceptFileIfWhitelistedAsync()
        {
            // Arrange
            var targetGuid = Guid.NewGuid();
            var contentAddingInfo = new ContentAddingInfo { Descriptor = new ContentDescriptor(targetGuid) };
            _mediaServerMock.Setup(ms => ms.AddMasterAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(contentAddingInfo));
            _mediaServerMock.SetupSupportedTypes(".mp4");

            var file = CreateFormFile("test.jpeg", "application/data", overwriteFilename: "test.mp4");

            // Act
            var actionResult = await _controller.AddMaster(file);

            // Assert
            var result = actionResult.Result as BadRequestObjectResult;
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
            Assert.That(result.Value, Is.EqualTo(targetGuid));
        }

        private FormFile CreateFormFile(string filename, string mimeType, string overwriteFilename = "")
            => FormFileBuilder.Load(Path.Combine(_testContentPath, filename), mimeType, overwriteFilename);

        [TestCase("test.jpg", "image/jpeg")]
        [TestCase("test.jpeg", "image/jpeg")]
        [TestCase("test.png", "image/png")]
        [TestCase("test.gif", "image/gif")]
        [TestCase("test.txt", "text/plain")]
        [TestCase("test.pdf", "application/pdf")]
        public async Task ShouldAcceptUploadOfTestVariantAsync(string fileName, string mimeType)
        {
            // Arrange
            var contentGuid = Guid.NewGuid();
            var targetGuid = Guid.NewGuid();
            var contentAddingInfo = new ContentAddingInfo { Descriptor = new ContentDescriptor(targetGuid) };
            _mediaServerMock.Setup(ms => ms.Get(It.Is<Guid>(g => g == contentGuid))).Returns(new ContentDescriptor(contentGuid));
            _mediaServerMock.Setup(ms => ms.AddVariantAsync(It.Is<Guid>(g => g == contentGuid), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(() => Task.FromResult(contentAddingInfo));

            using var stream = File.OpenRead(Path.Combine(_testContentPath, fileName));
            var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
            {
                Headers = new HeaderDictionary(),
                ContentType = mimeType
            };

            // Act
            var actionResult = await _controller.AddVariant(contentGuid.ToString(), "TestVariant", file);

            // Assert
            var okResult = actionResult.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult.StatusCode, Is.EqualTo(200));
            Assert.That(okResult.Value, Is.EqualTo(targetGuid));
        }

        [TestCase("11111111-1111-1111-1111-111111111111", "11111111-1111-1111-1111-111111111111", typeof(OkResult), Description = "Content successfully deleted")]
        [TestCase("11111111-1111-1111-1111-111111111111", "00000000-0000-0000-0000-000000000000", typeof(NotFoundObjectResult), Description = "Content not found")]
        [TestCase("11111111-1111-1111-1111-XXXXXXXXXXXX", "00000000-0000-0000-0000-000000000000", typeof(BadRequestResult), Description = "Invalid guid caused bad request result.")]
        public void ShouldRemoveContentById(string requestedGuid, string availableGuid, Type resultType)
        {
            // Arrange
            var parsedAvailableGuid = Guid.Parse(availableGuid);
            _mediaServerMock.Setup(ms => ms.DeleteContent(It.Is<Guid>(g => g == parsedAvailableGuid))).Returns(true);

            // Act
            var actionResult = _controller.RemoveContent(requestedGuid);

            // Assert
            Assert.That(actionResult.GetType().IsAssignableTo(resultType));
        }

        [TestCase("11111111-1111-1111-1111-111111111111", "11111111-1111-1111-1111-111111111111", typeof(OkResult), Description = "Variant successfully deleted")]
        [TestCase("11111111-1111-1111-1111-111111111111", "00000000-0000-0000-0000-000000000000", typeof(NotFoundObjectResult), Description = "Variant not found")]
        [TestCase("11111111-1111-1111-1111-XXXXXXXXXXXX", "00000000-0000-0000-0000-000000000000", typeof(BadRequestResult), Description = "Invalid guid caused bad request result.")]
        public void ShouldRemoveVariantByIdAndName(string requestedGuid, string availableGuid, Type resultType)
        {
            // Arrange
            var parsedAvailableGuid = Guid.Parse(availableGuid);
            _mediaServerMock.Setup(ms => ms.DeleteVariant(It.Is<Guid>(g => g == parsedAvailableGuid), It.IsAny<string>())).Returns(true);

            // Act
            var actionResult = _controller.RemoveVariant(requestedGuid, "TestVariantName");

            // Assert
            Assert.That(actionResult.GetType().IsAssignableTo(resultType));
        }
    }
}

