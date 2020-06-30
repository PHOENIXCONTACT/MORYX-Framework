// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;

namespace Moryx.Communication.Sockets.IntegrationTests
{
    public abstract class CommunicationSocketsTestsBase<TMessage> where TMessage : BinaryMessage
    {
        private List<ConnectionBuffer<TMessage>> _serverConnections;
        private List<ConnectionBuffer<TMessage>> _clients;
        private BinaryConnectionFactoryMock _binaryConnectionFactory;

        private List<ConnectionBuffer<TMessage>> _overallClients;

        private int _testPort;
        protected const string TestIpAdress = "127.0.0.1";

        protected List<ConnectionBuffer<TMessage>> ServerConnections => _serverConnections;
        protected List<ConnectionBuffer<TMessage>> Clients => _clients;

        protected int TestPort => _testPort;

        [OneTimeSetUp]
        public void SetUpTestCase()
        {
            var rnd = new Random();
            _testPort = rnd.Next(2000, 2101);

            _overallClients = new List<ConnectionBuffer<TMessage>>();
        }

        /// <summary>
        /// Gear up for brand new start for every TestCase.
        /// </summary>
        [SetUp]
        public void SetupTestCase()
        {
            _binaryConnectionFactory = new BinaryConnectionFactoryMock();
            _clients = new List<ConnectionBuffer<TMessage>>();
            _serverConnections = new List<ConnectionBuffer<TMessage>>();
        }

        [TearDown]
        public void TearDownTestCase()
        {
            CloseClients();
            CloseServer();

            _binaryConnectionFactory = null;
        }

        /// <summary>
        /// Get client by index
        /// </summary>
        protected ConnectionBuffer<TMessage> GetClient(int index) => _clients[index];

        /// <summary>
        /// Closes all server listeners
        /// </summary>
        protected void CloseServer()
        {
            foreach (var s in _serverConnections)
            {
                if (s.Connection == null)
                    return;

                s.Connection.Dispose();
                Assert.AreEqual(s.Connection.CurrentState, BinaryConnectionState.Disconnected,
                    "Server is not in the state '{0:G}'. CurrentState: {1:G}", BinaryConnectionState.Disconnected,
                    s.Connection.CurrentState);
                s.Connection = null;
            }
        }

        /// <summary>
        /// Closes all client connections
        /// </summary>
        protected void CloseClients()
        {
            //Clean up
            foreach (var c in _clients)
            {
                if (c.Connection == null)
                    return;

                c.Connection.Dispose();
                // Client should be disconnected 
                WaitForConnectionState(_clients.IndexOf(c), new TimeSpan(0, 0, 0, 10), BinaryConnectionState.Disconnected);
                c.Connection = null;
            }
        }

        /// <summary>
        /// Wait for a specified timeout that a client enters the <see cref="wantedState"/>
        /// </summary>
        /// <param name="clientIdx">Client to watch</param>
        /// <param name="timeToWait">Time out to wait</param>
        /// <param name="wantedState">The state the client shall enter</param>
        protected void WaitForConnectionState(int clientIdx, TimeSpan timeToWait, BinaryConnectionState wantedState)
        {
            Console.WriteLine($"WaitForConnectionState. ClientIdx: {clientIdx}, wanted state: {wantedState:G}");
            var timeout = DateTime.Now.Add(timeToWait);
            var start = DateTime.Now;
            var waitedFor = new TimeSpan();

            while (!_clients[clientIdx].Connection.CurrentState.Equals(wantedState) && timeout > DateTime.Now)
            {
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 100));
                waitedFor = DateTime.Now.Subtract(start);
            }

            // Client should be connected
            Assert.AreEqual(wantedState, _clients[clientIdx].Connection.CurrentState,
                $"Client ({clientIdx}) is not in the state '{wantedState:G}'. " +
                $"CurrentState: {_clients[clientIdx].Connection.CurrentState:G}. Waited for {waitedFor:g}");
        }

        /// <summary>
        /// Creates and starts a listener on the server. There has to be one listener for each clients that shall connect.
        /// </summary>
        /// <param name="ipAdress">The ip adress.</param>
        /// <param name="port">The port.</param>
        /// <param name="id">The identifier. This identifier has to be matched be the client that connects to this listener. The listener will be determined, when the first message is received</param>
        /// <param name="messageValidator">The message validator to be used</param>
        protected ConnectionBuffer<TMessage> CreateAndStartServer(IPAddress ipAdress, int port, int id, IMessageValidator messageValidator)
        {
            var server = new ConnectionBuffer<TMessage>
            {
                Id = id,
                Connection = _binaryConnectionFactory.Create(CreateServerConfig(ipAdress, port), messageValidator)
            };
            server.Connection.Received += (sender, message) => server.Received.Add((TMessage)message);
            server.Connection.NotifyConnectionState += (sender, message) => server.LastStateChangeEvents.Add(message);

            // Server should be disconnected
            Assert.AreEqual(server.Connection.CurrentState, BinaryConnectionState.Disconnected,
                $"server is not in the state '{BinaryConnectionState.Disconnected:G}'. CurrentState: {server.Connection.CurrentState:G}");

            server.Connection.Start();

            // Server should be listening 
            Assert.AreEqual(BinaryConnectionState.AttemptingConnection, server.Connection.CurrentState,
                $"server is not in the state '{BinaryConnectionState.AttemptingConnection:G}'. CurrentState: {server.Connection.CurrentState:G}");

            _serverConnections.Add(server);

            return server;
        }

        /// <summary>
        /// Creates and starts a client.
        /// </summary>
        /// <param name="ipAdress">The ip adress.</param>
        /// <param name="port">The port.</param>
        /// <param name="connectRetryWaitMs">The connect retry wait ms.</param>
        /// <param name="id">The identifier. This identifier has to be matched be the listener this client will connect to. The listener will be determined, when the first msg is send.</param>
        /// <returns></returns>
        protected int CreateAndStartClient(IPAddress ipAdress, int port, int connectRetryWaitMs, int id, IMessageValidator messageValidator)
        {
            var client = new ConnectionBuffer<TMessage>
            {
                Id = id,
                Connection = _binaryConnectionFactory.Create(CreateClientConfig(ipAdress, port, connectRetryWaitMs), messageValidator)
            };
            client.Connection.Received +=
                (sender, message) => client.Received.Add((TMessage)message);
            client.Connection.NotifyConnectionState += (sender, message) => client.LastStateChangeEvents.Add(message);

            var clientIdx = _clients.Count;
            _clients.Add(client);
            _overallClients.Add(client);

            Console.WriteLine("CreateAndStartClient Added Client idx: {0}.", clientIdx);

            // Client should be disconnected
            Assert.AreEqual(_clients[clientIdx].Connection.CurrentState, BinaryConnectionState.Disconnected,
                $"Client is not in the state '{BinaryConnectionState.Disconnected:G}'. CurrentState: {_clients[clientIdx].Connection.CurrentState:G}");

            _clients[clientIdx].Connection.Start();

            // In some tests the client shall be connected at this point and in some tests it shall not be connected, 
            // so check the connection-state somewhere else...

            return clientIdx;
        }

        /// <summary>
        /// Creates a message basing on SystemTestHeader.
        /// </summary>
        /// <param name="senderId">The sender id</param>
        /// <param name="payload">The payload</param>
        /// <returns>The binaray message</returns>
        protected virtual BinaryMessage CreateMessage(int senderId, byte[] payload)
        {
            return null;
        }

        /// <summary>
        /// Sends a number of messages
        /// </summary>
        /// <param name="numberOfClients">number of clients to send messages to</param>
        /// <param name="numberOfMessages">Number of messages</param>
        /// <param name="payloadMultiplier">Payload multiplier</param>
        /// <param name="connectionList">Connections to count received messages on</param>
        /// <param name="consoleText">Text that is written to console before sending payload</param>
        protected void SendMessages(int numberOfClients, int numberOfMessages, int payloadMultiplier, List<ConnectionBuffer<TMessage>> connectionList, string consoleText)
        {
            for (var i = 0; i < numberOfClients; i++)
            {
                for (var j = 0; j < numberOfMessages; j++)
                {
                    var i1 = i;
                    var j1 = j;
                    ThreadPool.QueueUserWorkItem(obj =>
                    {
                        var pl = CreatePayload(payloadMultiplier, BitConverter.GetBytes(i1));
                        Console.WriteLine(">>>{4:HH:mm:ss,ffff}-Sending Message for {0}: {1}, Message: {2}, MessageLength: {3}",
                            consoleText, i1, j1, pl.Length, DateTime.Now);

                        connectionList[i1].Connection.Send(CreateMessage(i1, pl));
                    });
                }
            }
        }

        /// <summary>
        /// Waits for a specific timeout that a number of messages has been received
        /// </summary>
        /// <param name="timeout">Timeout who long is waited to receive messages</param>
        /// <param name="numberOfMessages">Number of messages</param>
        /// <param name="connectionList">Connections to count received messages on</param>
        /// <returns>True if the parameterized amount of messages has been received otherwise false</returns>
        protected static bool WaitForMessageReception(TimeSpan timeout, int numberOfMessages, List<ConnectionBuffer<TMessage>> connectionList)
        {
            var now = DateTime.Now;
            var stop = now.Add(timeout);

            Console.WriteLine($"WaitForMessageReception: DateTime.Now: {now:HH:mm:ss.ffff}, Stopping at: {stop:HH:mm:ss.ffff}, TimeSpan: {timeout}");

            var allReceived = false;
            while (stop > DateTime.Now && !(allReceived = connectionList.All(s => s.Received.Count == numberOfMessages)))
            {
                Thread.Sleep(new TimeSpan(0, 0, 0, 0, 500));
            }

            Console.WriteLine($"WaitForMessageReception: allReceived: {allReceived}, DateTime.Now: {now:HH:mm:ss.ffff}");

            return allReceived;
        }

        /// <summary>
        /// Creates a payload with a multiplication of the given <see cref="element"/> array.
        /// </summary>
        /// <param name="counter">How often the element array shall be copied</param>
        /// <param name="element">The source payload</param>
        /// <returns>The created payload</returns>
        protected static byte[] CreatePayload(int counter, byte[] element)
        {
            var result = new byte[element.Length * counter];
            for (var i = 0; i < counter; i++)
            {
                element.CopyTo(result, i * element.Length);
            }
            return result;
        }

        /// <summary>
        /// Compares two messages with each other.
        /// </summary>
        /// <param name="msg1">The adress.</param>
        /// <param name="msg2">The port.</param>
        /// <returns>True if messages are equal otherwise false</returns>
        protected static bool CompareMessages(BinaryMessage<SystemTestHeader> msg1, BinaryMessage<SystemTestHeader> msg2)
        {
            Assert.IsNotNull(msg1, "Message 1 is null");
            Assert.IsNotNull(msg2, "Message 2 is null");

            Assert.AreEqual(msg1.Header.ClientIdx, msg2.Header.ClientIdx, "ClientIdxs do not match");

            Assert.AreEqual(msg1.Header.HeaderString, msg2.Header.HeaderString, "HeaderStrings do not match");

            Assert.AreEqual(msg1.Payload != null, msg2.Payload != null, "One Message has PayLoad, the other has not");

            if (msg1.Payload != null && msg2.Payload != null)
            {
                Assert.AreEqual(msg1.Payload.Length, msg2.Payload.Length, "Messages have different PayLoad-Length");
                for (var i = 0; i < msg1.Payload.Length; i++)
                {
                    Assert.AreEqual(msg1.Payload[i], msg2.Payload[i], "Messages-Payloads are different.");
                }
            }
            return true;
        }

        /// <summary>
        /// Creates the client configuration.
        /// </summary>
        /// <param name="adress">The adress.</param>
        /// <param name="port">The port.</param>
        /// <param name="connectRetryWaitMs">The connect retry wait ms.</param>
        /// <returns>The client configuration</returns>
        protected static TcpClientConfig CreateClientConfig(IPAddress adress, int port, int connectRetryWaitMs)
        {
            return new TcpClientConfig
            {
                IpAdress = adress.ToString(),
                Port = port,
                RetryWaitMs = connectRetryWaitMs,
                MonitoringIntervalMs = 100,
                MonitoringTimeoutMs = 500
            };
        }

        /// <summary>
        /// Creates the server configuration.
        /// </summary>
        /// <param name="adress">The adress.</param>
        /// <param name="port">The port.</param>
        /// <returns>The listener configuration</returns>
        protected static TcpListenerConfig CreateServerConfig(IPAddress adress, int port)
        {
            return new TcpListenerConfig
            {
                IpAdress = adress.ToString(),
                Port = port,
                MonitoringIntervalMs = 100,
                MonitoringTimeoutMs = 500
            };
        }
    }
}
