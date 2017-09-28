using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;

namespace Marvin.Communication.Sockets.IntegrationTests
{
    [TestFixture]
    public class CommunicationSocketsTests
    {

        private List<ConnectionBuffer> _serverConnections;
        private List<ConnectionBuffer> _clients;
        private BinaryConnectionFactoryMock _binaryConnectionFactory;

        private const int TestPort = 5010;
        private const string TestIpAdress = "127.0.0.1";

        #region Test setups & teardowns

        /// <summary>
        /// Gear up for brand new start for every TestCase.
        /// </summary>
        [SetUp]
        public void SetupTestCase()
        {
            Console.WriteLine("Test Setup");

            _binaryConnectionFactory = new BinaryConnectionFactoryMock();
            _clients = new List<ConnectionBuffer>();
            _serverConnections = new List<ConnectionBuffer>();
        }

        private void CloseServer()
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

        private void CloseClients()
        {
            //Clean up
            foreach (var c in _clients)
            {
                if (c.Connection == null)
                    return;

                c.Connection.Dispose();
                // Client should be disconnected 
                WaitForConnectionState(_clients.IndexOf(c), new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Disconnected);
                c.Connection = null;
            }
        }

        #endregion

        #region Header protocol tests

        /// <summary>
        /// Check simple connect and disconnect events without datatransfer.
        /// </summary>
        /// <param name="connectAndDisconnects">How many connects and disconnects per client.</param>
        /// <param name="simultaneousClients">How many clients should be connected simultaneously.</param>
        [TestCase(1, 1, Description = "ConnectAndDisconnects: 1, simultaneousClients: 1")]
        [TestCase(10, 1, Description = "ConnectAndDisconnects: 10, simultaneousClients: 1")]
        [TestCase(1, 10, Description = "ConnectAndDisconnects: 1, simultaneousClients: 10")]
        [TestCase(10, 10, Description = "ConnectAndDisconnects: 10, simultaneousClients: 10")]
        public void ConnectAndDisconnect(int connectAndDisconnects, int simultaneousClients)
        {
            Console.WriteLine("ConnectAndDisconnect.");

            for (int j = 0; j < simultaneousClients; j++)
            {
                CreateAndStartServer(IPAddress.Any, TestPort, j);
            }

            for (int i = 0; i < connectAndDisconnects; i++)
            {
                for (int j = 0; j < simultaneousClients; j++)
                {
                    int clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, -1, j);

                    // Client should be connected
                    WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 20), BinaryConnectionState.Connected);
                }

                for (int j = i * simultaneousClients; j < (i + 1) * simultaneousClients; j++)
                {
                    _clients[j].Connection.Dispose();
                    // Client should be disconnected 
                    WaitForConnectionState(j, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Disconnected);
                    _clients[j].Connection = null;
                }
            }
        }

        /// <summary>
        /// Connection-Attempts from clients without a listening server.
        /// </summary>
        /// <param name="numberOfClients">How many clients should attempt to connect simultaneously.</param>
        [TestCase(1, Description = "One client tries and fails to connect")]
        [TestCase(5, Description = "Ten clients try and fail to connect")]
        public void ClientsFailToConnect(int numberOfClients)
        {
            Console.WriteLine("ClientsFailToConnect.");

            for (int i = 0; i < numberOfClients; i++)
            {
                var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i);

                // Client should be attempting to Connect
                WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.AttemptingConnection);
            }

            Thread.Sleep(new TimeSpan(0, 0, 0, 5));

            for (int i = 0; i < numberOfClients; i++)
            {
                Assert.AreNotEqual(BinaryConnectionState.Connected, _clients[i].Connection.CurrentState,
                    "Client is in a connected-state, but there should be no server.");
            }
        }

        /// <summary>
        /// Send and receive data from client and server.
        /// </summary>
        /// <param name="numberOfClients">How many clients should be connected simultaneously.</param>
        /// <param name="numberOfMessages">How many mesages shall each client send/receive.</param>
        /// <param name="payloadMultiplier">The payload is one 32-Bit integer. to create large packets the payload length may multiplied by using this parameter.</param>
        [TestCase(1, 1, 0, Description = "One Client, sending 1 Message without PayLoad")]
        [TestCase(5, 100, 0, Description = "Five Clients, sending 100 Messages without PayLoad")]
        [TestCase(1, 1, 1, Description = "One Client, sending one message with 1 integers as PayLoad")]
        [TestCase(5, 100, 1, Description = "Five Clients, sending 100 Messages with 1 integers as PayLoad each")]
        [TestCase(1, 1, 20000, Description = "One Client, sending one message with 20.000 integers as PayLoad")]
        [TestCase(5, 10, 20000, Description = "Five Clients, sending 10 Messages with 20.000 integers as PayLoad each")]
        public void SendDataPacketsOnConnection(int numberOfClients, int numberOfMessages, int payloadMultiplier)
        {
            Console.WriteLine("SendDataPacketsOnConnection. numberOfClients {0}, numberOfMessages {1}, payloadMultiplier {2}",
                numberOfClients, numberOfMessages, payloadMultiplier);

            for (int i = 0; i < numberOfClients; i++)
            {
                CreateAndStartServer(IPAddress.Any, TestPort, i);

                var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i);

                // Client should be connected
                WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
            }

            //Send messages from the clients to the server
            SendMessages(numberOfClients, numberOfMessages, payloadMultiplier, _clients, "ClientIdx");

            //Two minute timeout for receiving Messages
            var timeout = new TimeSpan(0, 0, 2, 0);

            Console.WriteLine("Waiting for all client Messages to be send");
            var allReceived = WaitForMessageReception(timeout, numberOfMessages, _serverConnections);
            Assert.IsTrue(allReceived, "Not all ServerConnections received the right number of messages");
            Console.WriteLine("Waiting for sending the client messages complete");

            //Send messages from the server to clients
            SendMessages(numberOfClients, numberOfMessages, payloadMultiplier, _serverConnections, "_serverConnection");

            Console.WriteLine("Waiting for all server Messages to be send");
            allReceived = WaitForMessageReception(timeout, numberOfMessages, _clients);
            Assert.IsTrue(allReceived,"Not all Clients received the right number of messages");
            Console.WriteLine("Waiting for sending the server messages complete");

            //Check all received Messages
            for (int i = 0; i < numberOfClients; i++)
            {
                Assert.AreEqual(_serverConnections[i].Received.Count, _clients[i].Received.Count,
                    "Server and Client received a different number of messages");

                for (int j = 0; j < _serverConnections[i].Received.Count; j++)
                {
                    Console.WriteLine("CompareMessagesfor ServerConnection/ClientIdx: {0}, MessageNumber: {1}", i, j);
                    Assert.IsTrue(CompareMessages(_serverConnections[i].Received[j], _clients[i].Received[j]),
                        "Telegrams do not match");
                }
            }
        }

        public enum ShutdownType
        {
            ShutdownServer,
            ShutdownClient
        }

        /// <summary>
        /// The server or the clients disconnect while data is been transferred.
        /// </summary>
        /// <param name="numberOfClients">How many clients should be connected simultaneously.</param>
        /// <param name="shutdowntype">Shall the server disconnect first, or the clients.</param>
        [TestCase(1, ShutdownType.ShutdownClient, Description = "One Client SendDataPacketsWhileDisconnecting")]
        [TestCase(1, ShutdownType.ShutdownServer, Description = "One Client SendDataPacketsWhileDisconnecting")]
        [TestCase(2, ShutdownType.ShutdownClient, Description = "Two Client SendDataPacketsWhileDisconnecting")]
        [TestCase(2, ShutdownType.ShutdownServer, Description = "Two Client SendDataPacketsWhileDisconnecting")]
        public void SendDataPacketsWhileDisconnecting(int numberOfClients, ShutdownType shutdowntype)
        {
            Console.WriteLine("SendDataPacketsWhileDisconnecting. numberOfClients {0}", numberOfClients);

            for (int i = 0; i < numberOfClients; i++)
            {
                CreateAndStartServer(IPAddress.Any, TestPort, i);

                var clientIdx = CreateAndStartClient(IPAddress.Parse(TestIpAdress), TestPort, 500, i);

                // Client should be connected
                WaitForConnectionState(clientIdx, new TimeSpan(0, 0, 0, 5), BinaryConnectionState.Connected);
            }

            //Send messages from the clients to the server, to get the Server into the connected-state
            SendMessages(numberOfClients, 1, 1, _clients, "ClientIdx");

            var timeout = new TimeSpan(0, 0, 2, 0);
            Assert.IsTrue(WaitForMessageReception(timeout, 1, _serverConnections),
                "Not all ServerConnections received the right number of messages");

            //Reset Buffers
            _serverConnections.ForEach(sc => sc.Received.Clear());

            //Send messages from the clients to the server
            SendMessages(numberOfClients, 1, 25000000, _clients, "ClientIdx");

            //Send messages from the server to clients
            SendMessages(numberOfClients, 1, 25000000, _serverConnections, "_serverConnection");

            Thread.Sleep(new TimeSpan(0, 0, 0, 2));
            if (shutdowntype == ShutdownType.ShutdownClient)
            {
                CloseClients();
                CloseServer();
            }
            else
            {
                CloseServer();
                CloseClients();
            }

            _clients.ForEach(c => Assert.AreEqual(BinaryConnectionState.Disconnected, c.LastStateChangeEvent, "Client did not receive Disconnected-Event"));

            _serverConnections.ForEach(s => Assert.AreEqual(BinaryConnectionState.Disconnected, s.LastStateChangeEvent, "Serverconnection did not receive Disconnected-Event"));
        }

        private void SendMessages(int numberOfClients, int numberOfMessages, int payloadMultiplier, List<ConnectionBuffer> connectionList, string consoleText)
        {
            for (int i = 0; i < numberOfClients; i++)
            {
                for (int j = 0; j < numberOfMessages; j++)
                {
                    int i1 = i;
                    int j1 = j;
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

        private bool WaitForMessageReception(TimeSpan timeout, int numberOfMessages, List<ConnectionBuffer> connectionList)
        {
            var now = DateTime.Now;
            var stop = now.Add(timeout);

            Console.WriteLine($"WaitForMessageReception: DateTime.Now: {now:HH:mm:ss.ffff}, Stopping at: {stop:HH:mm:ss.ffff}, TimeSpan: {timeout}");

            var allReceived = false;
            while (stop > DateTime.Now && !(allReceived = connectionList.All(s => s.Received.Count == numberOfMessages)))
            {
                Thread.Sleep(new TimeSpan(0, 0, 0, 5));
            }

            Console.WriteLine($"WaitForMessageReception: allReceived: {allReceived}, DateTime.Now: {now:HH:mm:ss.ffff}");

            return allReceived;
        }

        #endregion

        #region Delimiter protocol tests

        #endregion

        #region Helper Methods

        private void WaitForConnectionState(int clientIdx, TimeSpan timeToWait, BinaryConnectionState wantedState)
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
                $"Client is not in the state '{wantedState:G}'. " +
                $"CurrentState: {_clients[clientIdx].Connection.CurrentState:G}. Waited for {waitedFor:g}");
        }

        /// <summary>
        /// Creates and starts a listener on the server. There has to be one listener for each clients that shall connect.
        /// </summary>
        /// <param name="ipAdress">The ip adress.</param>
        /// <param name="port">The port.</param>
        /// <param name="id">The identifier. This identifier has to be matched be the client that connects to this listener. The listener will be determined, when the first message is received</param>
        private void CreateAndStartServer(IPAddress ipAdress, int port, int id)
        {
            var server = new ConnectionBuffer
            {
                Id = id,
                Connection = _binaryConnectionFactory.Create(CreateServerConfig(ipAdress, port), new SystemTestValidator(id))
            };
            server.Connection.Received += (sender, message) => server.Received.Add((BinaryMessage<SystemTestHeader>)message);
            server.Connection.NotifyConnectionState += (sender, message) => server.LastStateChangeEvent = message;

            // Server should be disconnected
            Assert.AreEqual(server.Connection.CurrentState, BinaryConnectionState.Disconnected,
                $"server is not in the state '{BinaryConnectionState.Disconnected:G}'. CurrentState: {server.Connection.CurrentState:G}");

            server.Connection.Start();

            // Server should be listening 
            Assert.AreEqual(BinaryConnectionState.AttemptingConnection, server.Connection.CurrentState,
                $"server is not in the state '{BinaryConnectionState.AttemptingConnection:G}'. CurrentState: {server.Connection.CurrentState:G}");

            _serverConnections.Add(server);
        }

        /// <summary>
        /// Creates and starts a client.
        /// </summary>
        /// <param name="ipAdress">The ip adress.</param>
        /// <param name="port">The port.</param>
        /// <param name="connectRetryWaitMs">The connect retry wait ms.</param>
        /// <param name="id">The identifier. This identifier has to be matched be the listener this client will connect to. The listener will be determined, when the first msg is send.</param>
        /// <returns></returns>
        private int CreateAndStartClient(IPAddress ipAdress, int port, int connectRetryWaitMs, int id)
        {
            var client = new ConnectionBuffer
            {
                Id = id,
                Connection = _binaryConnectionFactory.Create(CreateClientConfig(ipAdress, port, connectRetryWaitMs), new SystemTestValidator(id))
            };
            client.Connection.Received +=
                (sender, message) => client.Received.Add((BinaryMessage<SystemTestHeader>)message);
            client.Connection.NotifyConnectionState += (sender, message) => client.LastStateChangeEvent = message;

            var clientIdx = _clients.Count;
            _clients.Add(client);

            Console.WriteLine("CreateAndStartClient Added Client idx: {0}.", clientIdx);

            // Client should be disconnected
            Assert.AreEqual(_clients[clientIdx].Connection.CurrentState, BinaryConnectionState.Disconnected,
                $"Client is not in the state '{BinaryConnectionState.Disconnected:G}'. CurrentState: {_clients[clientIdx].Connection.CurrentState:G}");

            _clients[clientIdx].Connection.Start();

            // In some tests the client shall be connected at this point and in some tests it shall not be connected, 
            // so check the connection-state somewhere else...

            return clientIdx;
        }

        private byte[] CreatePayload(int counter, byte[] element)
        {
            var result = new byte[element.Length * counter];
            for (int i = 0; i < counter; i++)
            {
                element.CopyTo(result, i * element.Length);
            }
            return result;
        }

        private BinaryMessage<SystemTestHeader> CreateMessage(int senderId, byte[] payload)
        {
            var header = new SystemTestHeader
            {
                ClientIdx = senderId,
                PayloadLength = payload.Length
            };

            return new BinaryMessage<SystemTestHeader>
            {
                Header = header,
                Payload = payload
            };
        }

        private bool CompareMessages(BinaryMessage<SystemTestHeader> msg1, BinaryMessage<SystemTestHeader> msg2)
        {
            Assert.IsNotNull(msg1, "Message 1 is null");
            Assert.IsNotNull(msg2, "Message 2 is null");

            Assert.AreEqual(msg1.Header.ClientIdx, msg2.Header.ClientIdx, "ClientIdxs do not match");

            Assert.AreEqual(msg1.Header.HeaderString, msg2.Header.HeaderString, "HeaderStrings do not match");

            Assert.AreEqual(msg1.Payload != null, msg2.Payload != null, "One Message has PayLoad, the other has not");

            if (msg1.Payload != null && msg2.Payload != null)
            {
                Assert.AreEqual(msg1.Payload.Length, msg2.Payload.Length, "Messages have different PayLoad-Length");
                for (int i = 0; i < msg1.Payload.Length; i++)
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
        /// <returns></returns>
        private static TcpClientConfig CreateClientConfig(IPAddress adress, int port, int connectRetryWaitMs)
        {
            //Console.WriteLine("CreateClientConfig.");
            return new TcpClientConfig
            {
                IpAdress = adress.ToString(),
                Port = port,
                RetryWaitMs = connectRetryWaitMs
            };
        }

        /// <summary>
        /// Creates the client configuration.
        /// </summary>
        /// <param name="adress">The adress.</param>
        /// <param name="port">The port.</param>
        /// <returns></returns>
        private static TcpListenerConfig CreateServerConfig(IPAddress adress, int port)
        {
            //Console.WriteLine("CreateServerConfig.");
            return new TcpListenerConfig
            {
                IpAdress = adress.ToString(),
                Port = port
            };
        }

        #endregion

        private class ConnectionBuffer
        {
            public ConnectionBuffer()
            {
                Received = new List<BinaryMessage<SystemTestHeader>>();
            }

            public IBinaryConnection Connection { get; set; }

            public List<BinaryMessage<SystemTestHeader>> Received { get; }

            public int Id { get; set; }

            public BinaryConnectionState LastStateChangeEvent { get; set; }
        }
    }
}