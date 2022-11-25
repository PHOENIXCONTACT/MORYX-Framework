// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;

namespace Moryx.Communication.Sockets.IntegrationTests
{
    [TestFixture]
    public class HeaderedCommunicationSocketTests : CommunicationSocketsTestsBase<BinaryMessage<SystemTestHeader>>
    {
        /// <summary>
        /// Send and receive data from client and server.
        /// </summary>
        /// <param name="numberOfClients">How many clients should be connected simultaneously.</param>
        /// <param name="numberOfMessages">How many messages shall each client send/receive.</param>
        /// <param name="payloadMultiplier">The payload is one 32-Bit integer. To create large packets the payload length may multiplied by using this parameter.</param>
        //[Test(Description = "Send and receive data from client and server")]
        //[TestCase(1, 1, 0, Description = "One Client, sending 1 Message without PayLoad")]
        //[TestCase(5, 100, 0, Description = "Five Clients, sending 100 Messages without PayLoad")]
        //[TestCase(1, 1, 1, Description = "One Client, sending one message with 1 integers as PayLoad")]
        //[TestCase(5, 100, 1, Description = "Five Clients, sending 100 Messages with 1 integers as PayLoad each")]
        //[TestCase(1, 1, 20000, Description = "One Client, sending one message with 20.000 integers as PayLoad")]
        //[TestCase(5, 10, 100, Description = "Five Clients, sending 10 Messages with 20.000 integers as PayLoad each")]
        //public void SendDataPacketsOnConnection(int numberOfClients, int numberOfMessages, int payloadMultiplier)
        //{
        //    // Arrange
        //    for (var i = 0; i < numberOfClients; i++)
        //    {
        //        CreateAndStartServer(IPAddress.Any, TestPort, i, new SystemTestValidator(i));

        //        var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i, new SystemTestValidator(i));

        //        // Client should be connected
        //        WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
        //    }

        //    // Act
        //    //Send messages from the clients to the server
        //    SendMessages(numberOfClients, numberOfMessages, payloadMultiplier, Clients, "ClientIdx");

        //    //Two minute timeout for receiving Messages
        //    var timeout = new TimeSpan(0, 0, 2, 0);

        //    // Assert
        //    Console.WriteLine("Waiting for all client Messages to be send");
        //    var allReceived = WaitForMessageReception(timeout, numberOfMessages, ServerConnections);
        //    Assert.IsTrue(allReceived, "Not all ServerConnections received the right number of messages");
        //    Console.WriteLine("Waiting for sending the client messages complete");

        //    //Send messages from the server to clients
        //    SendMessages(numberOfClients, numberOfMessages, payloadMultiplier, ServerConnections, "_serverConnection");

        //    Console.WriteLine("Waiting for all server Messages to be send");
        //    allReceived = WaitForMessageReception(timeout, numberOfMessages, Clients);
        //    Assert.IsTrue(allReceived, "Not all Clients received the right number of messages");
        //    Console.WriteLine("Waiting for sending the server messages complete");

        //    //Check all received Messages
        //    for (var i = 0; i < numberOfClients; i++)
        //    {
        //        Assert.AreEqual(ServerConnections[i].Received.Count, Clients[i].Received.Count,
        //            "Server and Client received a different number of messages");

        //        for (var j = 0; j < ServerConnections[i].Received.Count; j++)
        //        {
        //            Console.WriteLine("CompareMessagesfor ServerConnection/ClientIdx: {0}, MessageNumber: {1}", i, j);
        //            Assert.IsTrue(CompareMessages(ServerConnections[i].Received[j], Clients[i].Received[j]),
        //                "Telegrams do not match");
        //        }
        //    }
        //}

        public enum ShutdownType
        {
            ShutdownServer,
            ShutdownClient
        }

        /// <summary>
        /// Servers or clients disconnect while data is been transferred.
        /// </summary>
        /// <param name="numberOfClients">How many clients should be connected simultaneously.</param>
        /// <param name="shutdowntype">Shall the server disconnect first, or the clients.</param>
        //[Test(Description = "The server or the clients disconnect while data is been transferred.")]
        //[TestCase(1, ShutdownType.ShutdownClient, Description = "One client disconnects from server while data transfer")]
        //[TestCase(1, ShutdownType.ShutdownServer, Description = "One client is disconnected by server while data transfer")]
        //[TestCase(2, ShutdownType.ShutdownClient, Description = "Two clients disconnects from server while data transfer")]
        //[TestCase(2, ShutdownType.ShutdownServer, Description = "Two client are disconnected by server while data transfer")]
        //public void SendDataPacketsWhileDisconnecting(int numberOfClients, ShutdownType shutdowntype)
        //{
        //    // Arrange
        //    for (var i = 0; i < numberOfClients; i++)
        //    {
        //        CreateAndStartServer(IPAddress.Any, TestPort, i, new SystemTestValidator(i));

        //        var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i, new SystemTestValidator(i));

        //        // Client should be connected
        //        WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
        //    }

        //    //Send messages from the clients to the server, to get the Server into the connected-state
        //    SendMessages(numberOfClients, 1, 1, Clients, "ClientIdx");

        //    var timeout = new TimeSpan(0, 0, 2, 0);
        //    Assert.IsTrue(WaitForMessageReception(timeout, 1, ServerConnections),
        //        "Not all ServerConnections received the right number of messages");

        //    //Reset Buffers
        //    ServerConnections.ForEach(sc => sc.Received.Clear());

        //    // Act
        //    //Send messages from the clients to the server
        //    var clientEvents = SendMessages(numberOfClients, 1, 25000000, Clients, "ClientIdx");

        //    //Send messages from the server to clients
        //    var serverEvents = SendMessages(numberOfClients, 1, 25000000, ServerConnections, "_serverConnection");

        //    // Wait for each thread to initiate sending
        //    WaitHandle.WaitAll(clientEvents);
        //    WaitHandle.WaitAll(serverEvents);

        //    // Close connections
        //    if (shutdowntype == ShutdownType.ShutdownClient)
        //    {
        //        CloseClients();
        //        CloseServer();
        //    }
        //    else
        //    {
        //        CloseServer();
        //        CloseClients();
        //    }

        //    // Assert
        //    Clients.ForEach(c => Assert.AreEqual(BinaryConnectionState.Disconnected, c.LastStateChangeEvents.LastOrDefault(), "Client did not receive Disconnected-Event"));

        //    ServerConnections.ForEach(s => Assert.AreEqual(BinaryConnectionState.Disconnected, s.LastStateChangeEvents.LastOrDefault(), "Serverconnection did not receive Disconnected-Event"));
        //}

        //[TestCase(true, Description = "Server closes the connection upon receiving faulty message")]
        //[TestCase(false, Description = "Client closes the connection upon receiving faulty message")]
        //public void ReconnectAfterInvalidMessage(bool clientSendsMessage)
        //{
        //    // Arrange
        //    var server = CreateAndStartServer(IPAddress.Any, TestPort, 1, new SystemTestValidator(1));
        //    var clientId = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 100, 1, new SystemTestValidator(1));
        //    var client = GetClient(clientId);
        //    // Client should be connected
        //    WaitForConnectionState(clientId, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
        //    var stopWatch = new Stopwatch();
        //    stopWatch.Start();
        //    while (server.Connection.CurrentState != BinaryConnectionState.Connected && stopWatch.ElapsedMilliseconds < 5000)
        //        Thread.Sleep(1);

        //    // Act
        //    var binMessage = CreateMessage(42, new byte[] { 1, 3, 3, 7, 42 });
        //    if (clientSendsMessage)
        //        client.Connection.Send(binMessage);
        //    else
        //        server.Connection.Send(binMessage);
        //    Thread.Sleep(20);

        //    // Assert
        //    WaitForConnectionState(clientId, new TimeSpan(0, 0, 0, 20), BinaryConnectionState.Connected);
        //    Thread.Sleep(20);
        //    var connectionBuffer = clientSendsMessage ? server : client;
        //    Assert.AreEqual(0, connectionBuffer.Received.Count, "Server should not receive a message");
        //    var history = connectionBuffer.LastStateChangeEvents;
        //    Assert.LessOrEqual(4, history.Count);
        //    Assert.AreEqual(BinaryConnectionState.AttemptingConnection, history[history.Count - 2], "Server should have triggered a reconnect");
        //    Assert.AreEqual(BinaryConnectionState.Connected, history[history.Count - 1], "Server should have triggered a reconnect");
        //}

        /// <summary>
        /// Clients reconnect on a existant connection and sends messages
        /// </summary>
        /// <param name="numberOfClients">How many clients should be connected simultaneously.</param>
        /// <param name="numberOfMessages">Number of messages that are sent before reconnect and after reconnect</param>
        //[Test(Description = "Clients reconnect on a existant connection and sends messages")]
        //[TestCase(1, 200, Description = "1 Client reconnects, 200 messages")]
        //[TestCase(10, 200, Description = "10 Client reconnects, 200 messages")]
        //public void ReconnectingClientsWhenClientsConnected(int numberOfClients, int numberOfMessages)
        //{
        //    // Arrange
        //    for (var i = 0; i < numberOfClients; i++)
        //    {
        //        CreateAndStartServer(IPAddress.Any, TestPort, i, new SystemTestValidator(i));

        //        var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i, new SystemTestValidator(i));

        //        // Client should be connected
        //        WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

        //        Console.WriteLine("Connected: {0}", clientIdx);
        //    }

        //    // Act
        //    Console.WriteLine("Sending {0} messages", numberOfMessages);

        //    SendMessages(numberOfClients, numberOfMessages, 1, ServerConnections, "ClientIdx");
        //    Assert.IsTrue(WaitForMessageReception(new TimeSpan(0, 0, 0, 5), numberOfMessages, Clients));

        //    Console.WriteLine("Reconnecting all clients...");

        //    foreach (var client in Clients)
        //    {
        //        client.Connection.Reconnect();
        //        client.Received.Clear();

        //        // Client should be connected
        //        WaitForConnectionState(client.Id, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
        //    }

        //    Console.WriteLine("Sending {0} messages", numberOfMessages);

        //    SendMessages(numberOfClients, numberOfMessages, 1, Clients, "ClientIdx");
            
        //    // Assert
        //    Assert.IsTrue(WaitForMessageReception(new TimeSpan(0, 0, 0, 5), numberOfMessages, ServerConnections));
        //}

        /// <summary>
        /// Clients reconnect on a closed connection and sends messages
        /// </summary>
        /// <param name="numberOfClients">How many clients should be connected simultaneously.</param>
        /// <param name="numberOfMessages">Number of messages that are sent before reconnect and after reconnect</param>
        //[Test(Description = "Clients reconnect on a closed connection and sends messages")]
        //[TestCase(1, 200, Description = "1 Client reconnects, 200 messages")]
        //[TestCase(10, 200, Description = "10 Client reconnects, 200 messages")]
        //public void ReconnectingClientsWhenClientsDisconnected(int numberOfClients, int numberOfMessages)
        //{
        //    // Assert
        //    for (int i = 0; i < numberOfClients; i++)
        //    {
        //        CreateAndStartServer(IPAddress.Any, TestPort, i, new SystemTestValidator(i));

        //        var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i, new SystemTestValidator(i));

        //        // Client should be connected
        //        WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);

        //        Console.WriteLine("Connected: {0}", clientIdx);
        //    }

        //    // Act
        //    Console.WriteLine("Sending {0} messages", numberOfMessages);

        //    SendMessages(numberOfClients, numberOfMessages, 1, ServerConnections, "ClientIdx");
        //    Assert.IsTrue(WaitForMessageReception(new TimeSpan(0, 0, 0, 10), numberOfMessages, Clients));

        //    Console.WriteLine("Disconnecting all clients...");

        //    foreach (var client in Clients)
        //    {
        //        client.Connection.Stop();
        //        WaitForConnectionState(client.Id, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Disconnected);
        //    }

        //    Console.WriteLine("Reconnecting all clients...");

        //    foreach (var client in Clients)
        //    {
        //        client.Connection.Reconnect();
        //        client.Received.Clear();

        //        // Client should be connected
        //        WaitForConnectionState(client.Id, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
        //    }

        //    Console.WriteLine("Sending {0} messages", numberOfMessages);

        //    SendMessages(numberOfClients, numberOfMessages, 1, Clients, "ClientIdx");

        //    Thread.Sleep(2000);

        //    // Assert
        //    Assert.IsTrue(WaitForMessageReception(new TimeSpan(0, 0, 0, 10), numberOfMessages, ServerConnections));
        //}

        protected override BinaryMessage CreateMessage(int senderId, byte[] payload)
        {
            var header = new SystemTestHeader
            {
                ClientIdx = senderId,
                PayloadLength = payload.Length
            };

            return new BinaryMessage<SystemTestHeader>(header, payload);
        }
    }
}
