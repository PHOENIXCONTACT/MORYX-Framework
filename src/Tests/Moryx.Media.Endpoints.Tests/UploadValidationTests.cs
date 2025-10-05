// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Constraints;
using Moq;
using NUnit.Framework;
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Moryx.Media.Endpoints.Tests
{
    public class UploadValidationTests
    {
        private MediaServerController _controller;
        private Mock<IMediaServer> _mediaServerMock;
        private string _testContentPath;
        private string _errorMessage = "";
        private CultureInfo _culture;

        [OneTimeSetUp]
        public void SetupFixture()
        {
            _testContentPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "TestContent");
            _culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en");
        }

        [SetUp]
        public void Setup()
        {
            _mediaServerMock = new Mock<IMediaServer>();
            _controller = new MediaServerController(_mediaServerMock.Object);

            _mediaServerMock.SetupFileSize(10);
            _mediaServerMock.SetupSupportedTypes(".png", ".jpeg", ".jpg", ".gif", ".pdf", ".txt", ".csv");
        }

        [OneTimeTearDown]
        public void TearDownFixture()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
        }

        [Test]
        public void ZeroSizeFileIsRejected()
        {
            // Arrange
            _mediaServerMock.SetupSupportedTypes(".dat");

            var file = CreateFormFile("empty.dat", MimeTypeString.ApplicationData);

            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_errorMessage, Does.EndWith("empty.dat file size of 0 MB is to small!"));
        }

        [Test]
        public void ExceededFilesizeIsRejected()
        {
            // Arrange
            _mediaServerMock.SetupSupportedTypes(".dat");
            _mediaServerMock.SetupFileSize(1);

            using var stream = new MemoryStream();
            stream.SetLength(1024 * 1024 + 1);
            var file = FormFileBuilder.FromStream(stream, "exceeded.dat", MimeTypeString.ApplicationData);
            
            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_errorMessage, Is.EqualTo("exceeded.dat exceeds 1.0 MB."));
        }

        [Test]
        public void MaxFilesizeIsAccepted()
        {
            // Arrange
            _mediaServerMock.SetupSupportedTypes(".dat");
            _mediaServerMock.SetupFileSize(1);

            using var stream = new MemoryStream();
            stream.SetLength(1024 * 1024);
            var file = FormFileBuilder.FromStream(stream, "exceeded.dat", MimeTypeString.ApplicationData);

            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(result, Is.True);
        }

        [TestCase("bom-utf8.dat", 3)]
        [TestCase("bom-utf16be.dat", 2)]
        public void BomOnlyFileGetsRejected(string filename, int bomLength)
        {
            // Arrange
            _mediaServerMock.SetupSupportedTypes(".dat");

            var file = CreateFormFile(filename, MimeTypeString.ApplicationData);

            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(file.Length, Is.EqualTo(bomLength));
            Assert.That(result, Is.False);
            Assert.That(_errorMessage, Does.EndWith($"{filename} file size of 0 MB is to small!"));
        }

        [Test]
        public void NotSupportedFileGetsRejected()
        {
            // Arrange
            _mediaServerMock.SetupSupportedTypes(".dat");

            var file = CreateFormFile("test.jpg", MimeTypeString.ApplicationData);

            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_errorMessage, Does.EndWith($"test.jpg file type isn't permitted."));
        }

        [TestCase("test.jpg", "test.txt", "image/jpeg")]
        [TestCase("test.jpeg", "test.txt", "image/jpeg")]
        [TestCase("test.png", "test.txt", "image/png")]
        [TestCase("test.gif", "test.txt", "image/gif")]
        [TestCase("test.txt", "test.jpg", "text/plain")]
        [TestCase("test.pdf", "test.txt", "application/pdf")]
        public void WrongFileSignatureGetsRejected(string fileToLoad, string fileToPretend, string mimeType)
        {
            // Arrange
            var file = CreateFormFile(fileToLoad, mimeType, overwriteFilename: fileToPretend);

            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_errorMessage, Does.EndWith($"{fileToPretend} file signature doesn't match the file's extension."));
        }

        [Test]
        public void ExceptionReturnsValidationError()
        {
            // Arrange
            _mediaServerMock
                .Setup(ms => ms.GetSupportedFileTypes())
                .Throws(new ObjectDisposedException("SomeObject"));

            var file = CreateFormFile("test.txt", "text/plain");

            // Act
            var result = UploadValidation.ValidateFormFile(file, _mediaServerMock.Object, out _errorMessage);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(_errorMessage, Does.EndWith($"test.txt validation failed."));
        }

        private FormFile CreateFormFile(string filename, string mimeType, string overwriteFilename = "")
            => FormFileBuilder.Load(Path.Combine(_testContentPath, filename), mimeType, overwriteFilename);
    }

    public static class MediaServerMock
    {
        public static void SetupFileSize(this Mock<IMediaServer> mediaServer, int sizeInMb)
        { 
            mediaServer
                .Setup(ms => ms.FileSizeLimitInMb())
                .Returns(sizeInMb);
        }

        public static void SetupSupportedTypes(this Mock<IMediaServer> mediaServer, params string[] types)
        {
            mediaServer
                .Setup(ms => ms.GetSupportedFileTypes())
                .Returns(types);
        }
    }

    internal static class FormFileBuilder
    {
        internal static FormFile Load(string filename, string mimeType, string overwriteFilename = "")
        {
            var stream = File.OpenRead(filename);

            var formFilename = Path.GetFullPath(stream.Name);
            formFilename = string.IsNullOrEmpty(overwriteFilename)
                ? formFilename
                : formFilename.Replace(Path.GetFileName(formFilename), overwriteFilename);

            return new FormFile(stream, 0, stream.Length, null, formFilename)
            {
                Headers = new HeaderDictionary(),
                ContentType = mimeType
            };
        }

        internal static FormFile FromStream(Stream stream, string filename, string mimeType)
        {
            return new FormFile(stream, 0, stream.Length, null, filename)
            {
                Headers = new HeaderDictionary(),
                ContentType = mimeType
            };
        }
    }

    internal static class MimeTypeString
    {
        public const string ApplicationData = "application/data";
    }
}
