// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Net;
using NUnit.Framework;

namespace Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol
{
    [TestFixture]
    public class DelimiterCommunicationSocketTests : CommunicationSocketsTestsBase<BinaryMessage>
    {
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

        protected override BinaryMessage CreateMessage(int senderId, byte[] payload)
        {
            var stream = new List<byte>();
            stream.AddRange(TestDelimiterInterpreter.TestStartDelimiter);
            stream.AddRange(payload);
            stream.AddRange(EndDelimiterOnlyInterpreter.TestEndDelimiter);

            return new BinaryMessage { Payload = stream.ToArray() };
        }

        [Test(Description = "Sends a delimetered message and check the received data")]
        public void SendDelimitedMessage()
        {
            var payloadMultiplier = 10;

            // Arrange
            CreateAndStartServer(IPAddress.Any, TestPort, 0, new TestDelimiterValidator());

            var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAddress), TestPort, 500, 0, new TestDelimiterValidator());

            WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

            // Act
            SendMessages(1, 1, payloadMultiplier, ServerConnections, "ClientIdx");
            Assert.That(WaitForMessageReception(new TimeSpan(0, 0, 0, 5), 1, Clients));

            var payload = Clients[0].Received[0].Payload;

            BinaryMessage published = null;
            Array.Copy(payload, 0, _context.ReadBuffer, 0, payload.Length);
            _interpreter.ProcessReadBytes(_context, payload.Length, m => published = m);

            // Assert
            Assert.That(_context.StartFound, Is.False);
            Assert.That(_context.CurrentIndex, Is.EqualTo(0));

            Assert.That(published, Is.Not.Null);

            var expectedMsgLenth = TestDelimiterInterpreter.TestStartDelimiter.Length + payloadMultiplier * 4 +
                EndDelimiterOnlyInterpreter.TestEndDelimiter.Length;
            Assert.That(published.Payload.Length, Is.EqualTo(expectedMsgLenth));
        }
    }
}
