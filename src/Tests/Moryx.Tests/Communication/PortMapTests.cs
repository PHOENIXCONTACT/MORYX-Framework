// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Net;
using Moryx.Communication;
using Moryx.Communication.Sockets;
using NUnit.Framework;

namespace Moryx.Tests.Communication
{
    [TestFixture]
    public class PortMapTests
    {
        [TestCase("192.168.0.1", "192.168.0.2", false, true)]
        [TestCase("192.168.0.3", "192.168.0.3", false, false)]
        [TestCase("192.168.0.4", "192.168.0.4", true, true)]
        [TestCase("0.0.0.0", "192.168.0.5", false, false)]
        [TestCase("0.0.0.0", "192.168.0.6", true, true)]
        [TestCase("192.168.0.7", "0.0.0.0", false, false)]
        [TestCase("192.168.0.8", "0.0.0.0", true, true)]
        public void AddressValidation(string address1, string address2, bool sameProtocol, bool allowed)
        {
            // Arrange
            var protocol = new DummyProtocol(7);
            PortMap.Register(IPAddress.Parse(address1), 42, protocol);

            // Act
            var success = PortMap.Register(IPAddress.Parse(address2), 42, new DummyProtocol(sameProtocol ? 7 : 9));

            // Assert
            if (allowed)
                Assert.That(success, "Failed to register despite different address");
            else
                Assert.That(success, Is.False, "Should not allow registration");
        }

        [TestCase(13, 14, false, true)]
        [TestCase(15, 16, true, true)]
        [TestCase(17, 17, false, false)]
        [TestCase(18, 18, true, true)]
        public void PortValidation(int port1, int port2, bool sameProtocol, bool allowed)
        {
            // Arrange
            var protocol = new DummyProtocol(7);
            PortMap.Register(IPAddress.Parse("192.168.0.42"), port1, protocol);

            // Act
            var success = PortMap.Register(IPAddress.Parse("192.168.0.42"), port2, new DummyProtocol(sameProtocol ? 7 : 9));

            // Assert
            if (allowed)
                Assert.That(success, "Failed to register despite different address");
            else
                Assert.That(success, Is.False, "Should not allow registration");
        }

        private class DummyProtocol : IMessageInterpreter
        {
            private int ProtocolId { get; }

            public DummyProtocol(int protocolId)
            {
                ProtocolId = protocolId;
            }

            public bool Equals(IMessageInterpreter other)
            {
                return ProtocolId == (other as DummyProtocol)?.ProtocolId;
            }

            public IReadContext CreateContext()
            {
                throw new NotImplementedException();
            }

            public byte[] SerializeMessage(BinaryMessage message)
            {
                throw new NotImplementedException();
            }

            public ByteReadResult ProcessReadBytes(IReadContext context, int readBytes, Action<BinaryMessage> publishCompleteMessage)
            {
                throw new NotImplementedException();
            }

            public bool ErrorResponse(IReadContext context, out byte[] lastWill)
            {
                throw new NotImplementedException();
            }
        }
    }
}