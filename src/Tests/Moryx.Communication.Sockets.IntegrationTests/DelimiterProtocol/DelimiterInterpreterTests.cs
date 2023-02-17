// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    [TestFixture]
    public class DelimiterInterpreterTests
    {
        private const string Input = @"<html><h1>Hello</h1></html>";
        private const string Output = @"<html><h1>Hello</h1></html>";

        private IMessageInterpreter _interpreter;
        private DelimitedMessageContext _context;

        [SetUp]
        public void CaseSetup()
        {
            _interpreter = new TestDelimiterInterpreter();
            _context = (DelimitedMessageContext)_interpreter.CreateContext();

            // Extended properties 
            _context.StartFound = false;
        }

        [Test(Description = "Checks whether the base properties are set correctly after context creation")]
        public void CheckIfBaseDelimiterParametersAreCorrect()
        {
            // Arrange & Act
            var context = _context;

            // Base properties
            // Assert
            Assert.AreEqual(0, context.CurrentIndex);
            Assert.AreEqual(EndDelimiterOnlyInterpreter.TestReadSize, context.ReadSize);
            Assert.AreEqual(EndDelimiterOnlyInterpreter.TestBufferSize, context.ReadBuffer.Length);
        }

        [Test(Description = "Check if a partial received message was read correctly")]
        [TestCase(true, Description = "Use start and end delimiter")]
        [TestCase(false, Description = "Use only end delimiter")]
        public void ParsePartialDelimiterMessage(bool useStartDelimiter)
        {
            if (!useStartDelimiter)
                _interpreter = new EndDelimiterOnlyInterpreter();

            var text = Encoding.Unicode.GetBytes("Wie passend, du kämpfst");

            // Arrange
            var message = new List<byte>();
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(text);
            var readMessage = message.ToArray();

            // Act
            Array.Copy(readMessage, 0, _context.ReadBuffer, 0, readMessage.Length);
            _interpreter.ProcessReadBytes(_context, readMessage.Length, m => { });

            // Assert
            Assert.IsTrue(_context.StartFound);
            Assert.AreEqual(readMessage.Length, _context.CurrentIndex);
        }

        [Test(Description = "Check if full received message was parsed correctly")]
        [TestCase(0, Description = "No leading chunk, only the full message.")]
        [TestCase(4, Description = "4 bytes of leading chunk")]
        public void ParseFullDelimiterMessage(int leadingChunk)
        {
            var text = Encoding.Unicode.GetBytes("Wie passend, du kämpfst wie eine Kuh!");

            // Arrange
            var message = new List<byte>();
            for (var i = 1; i <= leadingChunk; i++)
            {
                message.Add((byte)i);
            }
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(text);
            message.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            var readMessage = message.ToArray();

            // Act
            BinaryMessage published = null;
            Array.Copy(readMessage, 0, _context.ReadBuffer, 0, readMessage.Length);
            _interpreter.ProcessReadBytes(_context, readMessage.Length, m => published = m);

            // Assert
            Assert.IsFalse(_context.StartFound);
            Assert.AreEqual(0, _context.CurrentIndex);

            Assert.NotNull(published);
            Assert.AreEqual(readMessage.Length - leadingChunk, published.Payload.Length);
            Assert.AreEqual(readMessage.Skip(leadingChunk).Sum(e => (short)e), published.Payload.Sum(e => (short)e));
        }

        [Test(Description = "Check wether a overlaaping received message was parsed correctly")]
        public void ParseOverlappingDelimiterMessage()
        {
            var text = Encoding.Unicode.GetBytes("Wie passend, du kämpfst wie eine Kuh!");
            var partialText = Encoding.Unicode.GetBytes("Wie passend");

            // Arrange
            var message = new List<byte>();
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(text);
            message.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            var fullmessage = message.ToArray();
            message.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            message.AddRange(partialText);
            var readMessage = message.ToArray();

            // Act
            BinaryMessage published = null;
            BinaryMessage notPublished = null;
            const int cut = EndDelimiterOnlyInterpreter.TestReadSize;

            Array.Copy(readMessage, 0, _context.ReadBuffer, 0, cut);
            _interpreter.ProcessReadBytes(_context, cut, m => published = m);

            var remain = readMessage.Length - cut;
            Array.Copy(readMessage, cut, _context.ReadBuffer, _context.CurrentIndex, remain);
            _interpreter.ProcessReadBytes(_context, remain, m => notPublished = m);

            // Assert
            Assert.IsTrue(_context.StartFound);
            Assert.AreEqual(readMessage.Length - fullmessage.Length, _context.CurrentIndex);
            // Check published message
            Assert.NotNull(published);
            Assert.AreEqual(fullmessage.Length, published.Payload.Length);
            Assert.AreEqual(fullmessage.Sum(e => (short)e), published.Payload.Sum(e => (short)e));
            // Check second was not published
            Assert.IsNull(notPublished);
        }

        [Test(Description = "Read message chunk wise")]
        [TestCase(1, Description = "Reading byte wise")]
        [TestCase(3, Description = "Chunks smaller than start or end")]
        [TestCase(41, Description = "Chunks of half a message")]
        [TestCase(82, Description = "Chunk matches first message")]
        [TestCase(EndDelimiterOnlyInterpreter.TestReadSize, Description = "Chunks of full read size")]
        public void ReadDelimiterMessageInChunks(int chunkSize)
        {
            var fullMessages = new byte[3][];

            // Arrange
            var stream = new List<byte>();
            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(Encoding.Unicode.GetBytes("Wie passend, du kämpfst wie eine Kuh!"));
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            fullMessages[0] = stream.ToArray();

            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(Encoding.Unicode.GetBytes("Geh mich ausm Weg oder ich schneid dir durch!"));
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            fullMessages[1] = stream.Skip(fullMessages[0].Length).ToArray();

            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(Encoding.Unicode.GetBytes("Mein Name ist Guybrush Threepwood und ich will Pirat werden!"));
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);
            fullMessages[2] = stream.Skip(fullMessages[0].Length).Skip(fullMessages[1].Length).ToArray();

            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            var readMessage = stream.ToArray();

            // Act
            var published = new List<BinaryMessage>();
            var index = 0;
            while (index < readMessage.Length)
            {
                var diff = readMessage.Length - index;
                var copyRange = diff > chunkSize ? chunkSize : diff;
                Array.Copy(readMessage, index, _context.ReadBuffer, _context.CurrentIndex, copyRange);
                _interpreter.ProcessReadBytes(_context, copyRange, published.Add);

                index += chunkSize;
            }

            // Assert
            Assert.IsTrue(_context.StartFound);
            Assert.AreEqual(4, _context.CurrentIndex);

            Assert.AreEqual(3, published.Count);
            for (var i = 0; i < 3; i++)
            {
                Assert.AreEqual(fullMessages[i].Length, published[i].Payload.Length);
                Assert.AreEqual(fullMessages[i].Sum(e => (short)e), published[i].Payload.Sum(e => (short)e));
            }
        }

        [Test(Description = "Parse html message")]
        public void ParseHtmlMessageAndUseHtmlInterpreter()
        {
            // Arrange
            _interpreter = HtmlInterpreter.Instance;
            _context = (DelimitedMessageContext)_interpreter.CreateContext();

            var text = Encoding.UTF8.GetBytes(Input);

            // Act
            BinaryMessage published = null;
            Array.Copy(text, 0, _context.ReadBuffer, _context.CurrentIndex, text.Length);
            _interpreter.ProcessReadBytes(_context, text.Length, m => published = m);

            // Assert
            Assert.NotNull(published);
            Assert.AreEqual(published.Payload, Encoding.UTF8.GetBytes(Output));
        }
    }
}
