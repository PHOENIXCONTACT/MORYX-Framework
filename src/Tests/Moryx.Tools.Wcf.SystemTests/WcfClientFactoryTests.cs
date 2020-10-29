// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moryx.TestTools.UnitTest;
using Moryx.Tools.Wcf.Tests.Logging;
using NUnit.Framework;

namespace Moryx.Tools.Wcf.Tests
{
    [TestFixture]
    public class WcfClientFactoryTests
    {
        private const int ShortWait = 1000;
        private const int MediumWait = 5000;

        private const string ClientId = "UnitTests";

        private const string LogMaintenanceServiceName = "ILogMaintenance";

        public enum ConnectionMode
        {
            New,
            Legacy
        }

        private VersionServiceManagerMock _versionServiceManager;
        private TestWcfClientFactory _wcfClientFactory;
        private ConnectionState _logMaintenanceState;
        private LogMaintenanceClientMock _logMaintenanceService;

        private readonly ManualResetEventSlim _logMaintenanceCallbackEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _moduleMaintenanceCallbackEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _connectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _disconnectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _allClientsConnectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _clientInfoEvent = new ManualResetEventSlim(false);

        private readonly List<WcfClientInfo> _receivedClientInfos = new List<WcfClientInfo>();

        [OneTimeSetUp]
        public void TestFixtureSetup()
        {
            _versionServiceManager = new VersionServiceManagerMock();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
        }

        [SetUp]
        public void Setup()
        {
            lock (_receivedClientInfos)
            {
                _receivedClientInfos.Clear();
            }

            _logMaintenanceCallbackEvent.Reset();
            _moduleMaintenanceCallbackEvent.Reset();
            _connectedEvent.Reset();
            _disconnectedEvent.Reset();
            _allClientsConnectedEvent.Reset();
            _clientInfoEvent.Reset();

            _versionServiceManager.EnableVersionService = true;
            _versionServiceManager.ServerVersion = "2.1.2";
            _versionServiceManager.Endpoint = "LogMaintenance";

            _wcfClientFactory = new TestWcfClientFactory
            {
                Logger = new DummyLogger(),
            };

            _wcfClientFactory.Initialize(new WcfClientFactoryConfig
            {
                ClientId = ClientId,
                Host = "localhost",
                Port = 80
            });
            _wcfClientFactory.VersionService = _versionServiceManager;

            _wcfClientFactory.ClientConnected += OnClientConnected;
            _wcfClientFactory.ClientDisconnected += OnClientDisconnected;
            _wcfClientFactory.AllClientsConnected += OnAllClientsConnected;
            _wcfClientFactory.ClientInfoChanged += OnClientInfoChanged;
        }

        private void OnClientConnected(object sender, string service)
        {
            _connectedEvent.Set();
        }

        private void OnClientDisconnected(object sender, string endpoint)
        {
            _disconnectedEvent.Set();
        }

        private void OnAllClientsConnected(object sender, EventArgs e)
        {
            _allClientsConnectedEvent.Set();
        }

        private void OnClientInfoChanged(object sender, WcfClientInfo clientInfo)
        {
            WcfClientInfo clone = clientInfo.Clone();

            lock (_receivedClientInfos)
            {
                _receivedClientInfos.Add(clone);
            }

            _clientInfoEvent.Set();
        }

        [TearDown]
        public void TearDown()
        {
            if (_wcfClientFactory != null)
            {
                _wcfClientFactory.ClientConnected -= OnClientConnected;
                _wcfClientFactory.ClientDisconnected -= OnClientDisconnected;
                _wcfClientFactory.AllClientsConnected -= OnAllClientsConnected;
                _wcfClientFactory.ClientInfoChanged -= OnClientInfoChanged;

                _wcfClientFactory.Dispose();

                _wcfClientFactory = null;
            }
        }

        [Test]
        public void TestClientId()
        {
            Assert.AreEqual(ClientId, _wcfClientFactory.ClientId);
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestClientInfo(ConnectionMode mode)
        {
            CreateLogClient(mode, "2.0.0.0");

            _connectedEvent.Wait(MediumWait);

            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial {0} client info event", LogMaintenanceServiceName);
            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final {0} client info event", LogMaintenanceServiceName);

            var info0 = _receivedClientInfos.First(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New);

            Assert.AreEqual("2.0.0.0", info0.ClientVersion, "ClientVersion before connect");
            Assert.AreEqual("2.0.0", info0.MinServerVersion, "MinServerVersion before connect");
            Assert.AreEqual("Unknown", info0.ServerVersion, "ServerVersion before connect");
            Assert.AreEqual("ILogMaintenance", info0.Service, "Service name before connect");
            Assert.AreEqual(mode == ConnectionMode.New ? "Unknown" : "http://localhost/LogMaintenance", info0.Uri, "Uri before connect");
            Assert.AreEqual(ConnectionState.New, info0.State, "State before connect");
            Assert.AreEqual(0, info0.Tries, "Tries before connect");

            var info1 = _receivedClientInfos.First(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Success);

            Assert.AreEqual("2.0.0.0", info1.ClientVersion, "ClientVersion after connect");
            Assert.AreEqual("2.0.0", info1.MinServerVersion, "MinServerVersion after connect");
            Assert.AreEqual(_versionServiceManager.ServerVersion, info1.ServerVersion, "ServerVersion after connect");
            Assert.AreEqual("ILogMaintenance", info1.Service, "Service name after connect");
            Assert.AreEqual(mode == ConnectionMode.New ? _versionServiceManager.ServiceUrl : "http://localhost/LogMaintenance", info1.Uri, "Uri after connect");
            Assert.AreEqual(ConnectionState.Success, info1.State, "State after connect");
            Assert.AreEqual(1, info1.Tries, "Tries after connect");

        }

        [TestCase("2.0")]
        [TestCase("2.1")]
        [TestCase("2.0.0")]
        [TestCase("2.0.9")]
        [TestCase("2.1.0")]
        // I know this looks ridiculous, but System.Versions CompareTo implementation is stupid
        public void CreateNewClientGood(string clientVersion)
        {
            CreateNewLogClient(clientVersion);

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.IsTrue(_connectedEvent.IsSet, "Not connected.");
            Assert.IsTrue(_allClientsConnectedEvent.IsSet, "Not all connected.");
            Assert.AreEqual(3, _receivedClientInfos.Count);

            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial {0} client info event", LogMaintenanceServiceName);
            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate {0} client info event", LogMaintenanceServiceName);
            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final {0} client info event", LogMaintenanceServiceName);
        }

        [Test]
        public void TestCreateLegacyClientWithoutVersionCheck()
        {
            var clientConfig = new ClientConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "LogMaintenance",
                Host = "localhost",
                Port = 80,
            };

            _versionServiceManager.EnableVersionService = false;

            _wcfClientFactory.Create<LogMaintenanceClientMock, ILogMaintenance>(clientConfig, LogMaintenanceCallback);

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.IsTrue(_connectedEvent.IsSet, "Not connected.");
            Assert.IsTrue(_allClientsConnectedEvent.IsSet, "Not all connected.");
            Assert.AreEqual(3, _receivedClientInfos.Count);

            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial {0} client info event", LogMaintenanceServiceName);
            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate {0} client info event", LogMaintenanceServiceName);
            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final {0} client info event", LogMaintenanceServiceName);
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestTryConnect(ConnectionMode mode)
        {
            _versionServiceManager.EnableVersionService = false;

            CreateLogClient(mode, "2.0.0.0");

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.IsFalse(_connectedEvent.IsSet, "Connected.");
            Assert.IsFalse(_allClientsConnectedEvent.IsSet, "All connected.");

            Assert.Null(_logMaintenanceService, "Got service");
            Assert.AreEqual(ConnectionState.FailedTry, _logMaintenanceState, "Service state");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial LogMaintenanceServiceName client info event");
                Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate LogMaintenanceServiceName client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.FailedTry && i.Tries == 1), "Received first failed LogMaintenanceServiceName client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.FailedTry && i.Tries == 2), "Received second LogMaintenanceServiceName client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.FailedTry && i.Tries == 3), "Received third LogMaintenanceServiceName client info event");
            }

            _versionServiceManager.EnableVersionService = true;

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.IsTrue(_connectedEvent.IsSet, "Not connected.");
            Assert.IsTrue(_allClientsConnectedEvent.IsSet, "Not all connected.");

            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final {0} client info event", LogMaintenanceServiceName);
        }

        [TestCase(ConnectionMode.New, false)]
        [TestCase(ConnectionMode.New, true)]
        [TestCase(ConnectionMode.Legacy, false)]
        [TestCase(ConnectionMode.Legacy, true)]
        public void TestDestroyClient(ConnectionMode mode, bool enableConnection)
        {
            _versionServiceManager.EnableVersionService = enableConnection;

            long clientId = CreateLogClient(mode, "2.0.0.0");

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.AreEqual(enableConnection, _connectedEvent.IsSet, "Connected.");
            Assert.AreEqual(enableConnection, _allClientsConnectedEvent.IsSet, "All connected.");

            _wcfClientFactory.Destroy(clientId);

            lock (_receivedClientInfos)
            {
                _receivedClientInfos.Clear();
            }

            _disconnectedEvent.Reset();

            _disconnectedEvent.Wait(MediumWait);

            Assert.AreEqual(enableConnection, _disconnectedEvent.IsSet, "Disconnected.");

            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Closing), "Received LogMaintenanceServiceName Closing event");

            if (enableConnection)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Closed), "Received LogMaintenanceServiceName Closed event");
            }
            else
            {
                Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Closed), "Received LogMaintenanceServiceName Closed event");
            }

            Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State != ConnectionState.Closing && i.State != ConnectionState.Closed), "Received other LogMaintenanceServiceName client info event");
        }

        [Test]
        public void TestDestroyNonExistingClient()
        {
            Assert.Throws<InvalidOperationException>(() => _wcfClientFactory.Destroy(-1));
        }

        [TestCase(ConnectionMode.New, "1.6.0.0", "2.0.0.0")]
        [TestCase(ConnectionMode.New, "2.3.0.0", "2.0.0.0")]
        [TestCase(ConnectionMode.Legacy, "1.8.0.0", "1.6.0.0")]
        [TestCase(ConnectionMode.Legacy, "3.0.0.0", "2.4.0.0")]
        public void TestVersionMismatch(ConnectionMode mode, string clientVersion, string serverVersion)
        {
            _versionServiceManager.ServerVersion = serverVersion;

            CreateLogClient(mode, clientVersion);

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.IsFalse(_connectedEvent.IsSet, "Connected.");
            Assert.IsFalse(_allClientsConnectedEvent.IsSet, "All connected.");

            Assert.Null(_logMaintenanceService, "Got service");
            Assert.AreEqual(ConnectionState.VersionMissmatch, _logMaintenanceState, "Service state");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial LogMaintenanceServiceName client info event");
                Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate LogMaintenanceServiceName client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.VersionMissmatch && i.Tries == 1), "Received first failed LogMaintenanceServiceName client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.VersionMissmatch && i.Tries == 2), "Received second LogMaintenanceServiceName client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.VersionMissmatch && i.Tries == 3), "Received third LogMaintenanceServiceName client info event");
            }

            _versionServiceManager.ServerVersion = clientVersion;

            _connectedEvent.Wait(MediumWait);
            _allClientsConnectedEvent.Wait(ShortWait);

            Assert.IsTrue(_connectedEvent.IsSet, "Not connected.");
            Assert.IsTrue(_allClientsConnectedEvent.IsSet, "Not all connected.");

            Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == LogMaintenanceServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final {0} client info event", LogMaintenanceServiceName);
        }

        private long CreateLogClient(ConnectionMode mode, string clientVersion)
        {
            switch (mode)
            {
                case ConnectionMode.New:
                    return CreateNewLogClient(clientVersion);

                case ConnectionMode.Legacy:
                    return CreateLegacyLogClient(clientVersion);

                default:
                    Assert.Fail("Unknown connection mode '{0}'", mode);
                    break;
            }

            return 0;
        }

        private long CreateNewLogClient(string clientVersion)
        {
            return _wcfClientFactory.Create<LogMaintenanceClientMock, ILogMaintenance>(clientVersion, LogMaintenanceCallback);
        }

        private long CreateLegacyLogClient(string clientVersion)
        {
            ClientConfig clientConfig = new ClientConfig
            {
                BindingType = ServiceBindingType.BasicHttp,
                Endpoint = "LogMaintenance",
                Host = "localhost",
                Port = 80,
                ClientVersion = clientVersion,
                CheckVersion = true
            };

            return _wcfClientFactory.Create<LogMaintenanceClientMock, ILogMaintenance>(clientConfig, LogMaintenanceCallback);
        }

        private void LogMaintenanceCallback(ConnectionState state, LogMaintenanceClientMock service)
        {
            _logMaintenanceState = state;
            _logMaintenanceService = service;

            _logMaintenanceCallbackEvent.Set();
        }
    }
}
