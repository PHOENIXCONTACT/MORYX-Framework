// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Net;
using System.Threading;
using Moryx.Communication.Sockets.IntegrationTests.DelimiterProtocol;
using NUnit.Framework;

namespace Moryx.Communication.Sockets.IntegrationTests
{
    [TestFixture]
    public class CommunicationSocketsTests : CommunicationSocketsTestsBase<BinaryMessage<SystemTestHeader>>
    {
        /// <summary>
        /// Check simple connect and disconnect events without datatransfer.
        /// </summary>
        /// <param name="connectAndDisconnects">How many connects and disconnects per client.</param>
        /// <param name="simultaneousClients">How many clients should be connected simultaneously.</param>
        [Test(Description = "Check simple connect and disconnect events without datatransfer.")]
        [TestCase(1, 1, Description = "ConnectAndDisconnects: 1, simultaneousClients: 1")]
        [TestCase(10, 1, Description = "ConnectAndDisconnects: 10, simultaneousClients: 1")]
        [TestCase(1, 10, Description = "ConnectAndDisconnects: 1, simultaneousClients: 10")]
        [TestCase(10, 10, Description = "ConnectAndDisconnects: 10, simultaneousClients: 10")]
        public void ConnectAndDisconnectWithoutDataTransfer(int connectAndDisconnects, int simultaneousClients)
        {
            for (var j = 0; j < simultaneousClients; j++)
            {
                CreateAndStartServer(IPAddress.Any, TestPort, j, new SystemTestValidator(j));
            }

            for (var i = 0; i < connectAndDisconnects; i++)
            {
                for (var j = 0; j < simultaneousClients; j++)
                {
                    var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, -1, j, new SystemTestValidator(j));

                    // Client should be connected
                    WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 20), BinaryConnectionState.Connected);
                }

                for (var j = i * simultaneousClients; j < (i + 1) * simultaneousClients; j++)
                {
                    Clients[j].Connection.Dispose();
                    // Client should be disconnected 
                    WaitForConnectionState(j, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Disconnected);
                    Clients[j].Connection = null;
                }
            }
        }

        /// <summary>
        /// Connection-Attempts from clients without a listening server.
        /// </summary>
        /// <param name="numberOfClients">How many clients should attempt to connect simultaneously.</param>
        [Test(Description = "Connection-Attempts from clients without a listening server")]
        [TestCase(1, Description = "One client tries and fails to connect")]
        [TestCase(5, Description = "Ten clients try and fail to connect")]
        public void ClientsFailToConnect(int numberOfClients)
        {
            for (var i = 0; i < numberOfClients; i++)
            {
                var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i, new SystemTestValidator(i));

                // Client should be attempting to Connect
                WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.AttemptingConnection);
            }

            Thread.Sleep(new TimeSpan(0, 0, 0, 5));

            for (int i = 0; i < numberOfClients; i++)
            {
                Assert.AreNotEqual(BinaryConnectionState.Connected, Clients[i].Connection.CurrentState,
                    "Client is in a connected-state, but there should be no server.");
            }
        }

        [Test(Description = "Tries serveral unsuccessful reconnects and one successful one")]
        public void ReconnectTest()
        {
            // Arrange
            var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, -1, 0, new SystemTestValidator(0));

            // Act
            // Assert
            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.AttemptingConnection);
            
            for (var i = 0; i < 10; i++)
            {
                Clients[clientIdx].Connection.Reconnect();
                WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.AttemptingConnection);
            }

            // Start server and await first connection
            var server = CreateAndStartServer(IPAddress.Any, TestPort, 0, new SystemTestValidator(0));
            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

            // Close connection from client side and await new connection
            Clients[clientIdx].Connection.Reconnect();
            Thread.Sleep(50);
            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

            // Close connection from server side and await new connection
            server.Connection.Reconnect();
            Thread.Sleep(50);
            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

            Clients[clientIdx].Connection.Stop();

            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Disconnected);
        }

        [Test]
        public void SendEmptyMessages()
        {
            var server = CreateAndStartServer(IPAddress.Any, TestPort + 1, 1, new TestDelimiterValidator());
            CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort + 1, -1, 1, new TestDelimiterValidator());
            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

            var message = new BinaryMessage(new byte[0]);
            Clients[0].Connection.Send(message);
            server.Connection.Send(message);

            // Make sure server does not interpret empty package as disconnect
            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

            CloseServer();

            WaitForConnectionState(0, new TimeSpan(0, 0, 0, 1), BinaryConnectionState.AttemptingConnection);
        }
    }
}
