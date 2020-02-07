// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Moryx.DependentTestModule;
using Moryx.Runtime.Maintenance.Modules;
using Moryx.Runtime.Modules;
using Moryx.Runtime.SystemTests;
using Moryx.Tools.Wcf.SystemTests.HelloWorld;
using Moryx.Tools.Wcf.SystemTests.SimpleHelloWorld;
using Moryx.Serialization;
using Moryx.TestModule;
using Moryx.TestTools.SystemTest;
using Moryx.TestTools.UnitTest;
using NUnit.Framework;
using IHelloWorldWcfService = Moryx.Tools.Wcf.SystemTests.HelloWorld.IHelloWorldWcfService;
using IHelloWorldWcfServiceCallback = Moryx.Tools.Wcf.SystemTests.HelloWorld.IHelloWorldWcfServiceCallback;
using ISimpleHelloWorldWcfService = Moryx.Tools.Wcf.SystemTests.SimpleHelloWorld.ISimpleHelloWorldWcfService;
using ModuleController = Moryx.DependentTestModule.ModuleController;

namespace Moryx.Tools.Wcf.SystemTests
{
    [TestFixture]
    public class ClientFactoryTests : IDisposable
    {
        private const int ShortWait = 1000;
        private const int MediumWait = 5000;
        private const int LongWait = 10000;

        public enum ConnectionMode
        {
            New,
            Legacy,
            LegacyWithHost
        }

        private HeartOfGoldController _hogController;
        private SimpleWcfClientFactory _clientFactory;

        private const string NetTcpServiceName = "IHelloWorldWcfService";
        private const string BasicHttpServiceName = "ISimpleHelloWorldWcfService";

        private const string NetTcpEndpointName = "HelloWorldWcfService";
        private const string BasicHttpEndpointName = "SimpleHelloWorldWcfService";

        private readonly ManualResetEventSlim _netTcpConnectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _netTcpDisconnectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _basicHttpConnectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _basicHttpDisconnectedEvent = new ManualResetEventSlim(false);
        private readonly ManualResetEventSlim _clientInfoChangedEvent = new ManualResetEventSlim(false);
        private static readonly ManualResetEventSlim NetTcpCallbackReceived = new ManualResetEventSlim(false);
        private static readonly ManualResetEventSlim NetTcpThrowCallbackReceived = new ManualResetEventSlim(false);

        private ConnectionState _basicHttpState;
        private SimpleHelloWorldWcfServiceClient _basicHttpService;

        private readonly IHelloWorldWcfServiceCallback _netTcpCallback = new HelloWorldWcfServiceCallback();
        private ConnectionState _netTcpState;
        private HelloWorldWcfServiceClient _netTcpService;

        private readonly List<WcfClientInfo> _receivedClientInfos = new List<WcfClientInfo>();

        private static string _receivedMessage;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 600
            };

            Console.WriteLine("Starting HeartOfGold");

            bool started = _hogController.StartHeartOfGold();
            _hogController.CreateClients();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            bool result = _hogController.WaitForService(ModuleController.ModuleName, ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");
        }

        /// <summary>
        /// Shut down the system test.
        /// </summary>
        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Trying to stop HeartOfGold");

                _hogController.StopHeartOfGold(10);

                if (!_hogController.Process.HasExited)
                {
                    Console.WriteLine("Killing HeartOfGold");
                    _hogController.Process.Kill();

                    Thread.Sleep(1000);

                    Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
                }
            }
        }

        public void Dispose()
        {
            if (_hogController != null)
            {
                _hogController.Dispose();
                _hogController = null;
            }
        }

        [SetUp]
        public void SetUp()
        {
            Clear(true);

            _clientFactory = new SimpleWcfClientFactory
            {
                Logger = new DummyLogger()
            };

            _clientFactory.Initialize(new WcfClientFactoryConfig
            {
                ClientId = "SystemTests",
                Host = "localhost",
                Port = _hogController.HttpPort
            });

            _clientFactory.ClientConnected += OnClientConnected;
            _clientFactory.ClientDisconnected += OnClientDisconnected;
            _clientFactory.ClientInfoChanged += OnClientInfoChanged;
        }

        [TearDown]
        public void TearDown()
        {
            SetBindingType(BindingType.BasicHttp);

            if (_clientFactory != null)
            {
                _clientFactory.ClientConnected -= OnClientConnected;
                _clientFactory.ClientDisconnected -= OnClientDisconnected;
                _clientFactory.ClientInfoChanged -= OnClientInfoChanged;

                _clientFactory.Dispose();

                _clientFactory = null;
            }

            _hogController.StartService("TestModule");
            bool result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running' ");

            _hogController.StartService("DependentTestModule");
            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running' ");
        }

        private void OnClientConnected(object sender, string service)
        {
            switch (service)
            {
                case NetTcpServiceName:
                    _netTcpConnectedEvent.Set();
                    break;

                case BasicHttpServiceName:
                    _basicHttpConnectedEvent.Set();
                    break;
            }
        }

        private void OnClientDisconnected(object sender, string endpoint)
        {
            switch (endpoint)
            {
                case NetTcpServiceName:
                    _netTcpDisconnectedEvent.Set();
                    break;

                case BasicHttpServiceName:
                    _basicHttpDisconnectedEvent.Set();
                    break;
            }
        }

        private void OnClientInfoChanged(object sender, WcfClientInfo clientInfo)
        {
            WcfClientInfo clone = clientInfo.Clone();

            lock (_receivedClientInfos)
            {
                _receivedClientInfos.Add(clone);
            }
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        [TestCase(ConnectionMode.LegacyWithHost)]
        public void TestConnect(ConnectionMode mode)
        {
            Connect(mode);

            _netTcpConnectedEvent.Wait(MediumWait);
            _basicHttpConnectedEvent.Wait(ShortWait);

            Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");
            Assert.IsTrue(_basicHttpConnectedEvent.IsSet, "No ClientConnected received for ISimpleHelloWorldWcfService");

            Assert.NotNull(_basicHttpService, "Didn't get SimpleHelloWorld service");
            Assert.AreEqual(ConnectionState.Success, _basicHttpState, "SimpleHelloWorld state");

            Assert.NotNull(_netTcpService, "Didn't get HelloWorld service");
            Assert.AreEqual(ConnectionState.Success, _netTcpState, "HelloWorld state");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final IHelloWorldWcfService client info event");

                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final ISimpleHelloWorldWcfService client info event");
            }
        }

        [TestCase(ConnectionMode.New, "DependentTestModule", false)]
        [TestCase(ConnectionMode.New, "TestModule", true)]
        [TestCase(ConnectionMode.Legacy, "DependentTestModule", false)]
        [TestCase(ConnectionMode.Legacy, "TestModule", true)]
        public void TestConnect(ConnectionMode mode, string module, bool expectEvents)
        {
            _hogController.StopService(module);
            bool result = _hogController.WaitForService(module, ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", module);

            Connect(mode);

            _basicHttpDisconnectedEvent.Wait(MediumWait);
            _netTcpDisconnectedEvent.Wait(ShortWait);
            _netTcpConnectedEvent.Wait(ShortWait);

            lock (_receivedClientInfos)
            {
                if (expectEvents)
                {
                    Assert.IsTrue(_netTcpDisconnectedEvent.IsSet, "No ClientDisconnected received for IHelloWorldWcfService");
                    Assert.IsFalse(_netTcpConnectedEvent.IsSet, "ClientConnected received for IHelloWorldWcfService");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received IHelloWorldWcfService client info event 'New'");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries == 1), "Received IHelloWorldWcfService client info event 'FailedTry'");
                }
                else
                {
                    Assert.IsFalse(_netTcpDisconnectedEvent.IsSet, "ClientDisconnected received for IHelloWorldWcfService");
                    Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial IHelloWorldWcfService client info event");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate IHelloWorldWcfService client info event");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final IHelloWorldWcfService client info event");
                }

                Assert.IsTrue(_basicHttpDisconnectedEvent.IsSet, "No ClientDisconnected received for ISimpleHelloWorldWcfService");

                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received ISimpleHelloWorldWcfService client info event 'New'");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.FailedTry && i.Tries == 1), "Received ISimpleHelloWorldWcfService client info event 'FailedTry'");
            }

            Clear(true);

            _hogController.StartService("DependentTestModule");
            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running'");

            _netTcpConnectedEvent.Wait(LongWait);
            _basicHttpConnectedEvent.Wait(ShortWait);

            lock (_receivedClientInfos)
            {
                if (expectEvents)
                {
                    Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries > 1), "Received IHelloWorldWcfService client info event 'FailedTry'");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received IHelloWorldWcfService client info event 'Success'");
                }
                else
                {
                    Assert.IsFalse(_netTcpConnectedEvent.IsSet, "ClientConnected received for IHelloWorldWcfService");
                    Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName), "Received initial IHelloWorldWcfService client info event");
                }

                Assert.IsTrue(_basicHttpConnectedEvent.IsSet, "No ClientConnected received for ISimpleHelloWorldWcfService");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final ISimpleHelloWorldWcfService client info event");
            }
        }

        [TestCase(ConnectionMode.New, "DependentTestModule", false)]
        [TestCase(ConnectionMode.New, "TestModule", true)]
        [TestCase(ConnectionMode.Legacy, "DependentTestModule", false)]
        [TestCase(ConnectionMode.Legacy, "TestModule", true)]
        public void TestReconnect(ConnectionMode mode, string module, bool expectEvents)
        {
            TestConnect(mode);

            Clear(true);

            _hogController.StopService(module);
            bool result = _hogController.WaitForService(module, ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", module);

            _basicHttpDisconnectedEvent.Wait(LongWait);
            _netTcpDisconnectedEvent.Wait(ShortWait);

            lock (_receivedClientInfos)
            {
                if (expectEvents)
                {
                    Assert.IsTrue(_netTcpDisconnectedEvent.IsSet, "No ClientDisconnected received for IHelloWorldWcfService");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.ConnectionLost && i.Tries == 1), "Received IHelloWorldWcfService client info event 'ConnectionLost'");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries == 1), "Received IHelloWorldWcfService client info event 'FailedTry'");
                }
                else
                {
                    Assert.IsFalse(_netTcpDisconnectedEvent.IsSet, "ClientDisconnected received for IHelloWorldWcfService");
                    Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName), "Received initial IHelloWorldWcfService client info event");
                }

                Assert.IsFalse(_basicHttpDisconnectedEvent.IsSet, "ClientDisconnected received for ISimpleHelloWorldWcfService");

                Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName), "Received initial ISimpleHelloWorldWcfService client info event");
            }

            Clear(true);

            _hogController.StartService("DependentTestModule");
            result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running'");

            _netTcpConnectedEvent.Wait(LongWait);
            _basicHttpConnectedEvent.Wait(ShortWait);

            lock (_receivedClientInfos)
            {
                if (expectEvents)
                {
                    Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries > 1), "Received IHelloWorldWcfService client info event 'FailedTry'");
                    Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received IHelloWorldWcfService client info event 'Success'");
                }
                else
                {
                    Assert.IsFalse(_netTcpConnectedEvent.IsSet, "ClientConnected received for IHelloWorldWcfService");
                    Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName), "Received initial IHelloWorldWcfService client info event");
                }

                Assert.IsFalse(_basicHttpConnectedEvent.IsSet, "ClientConnected received for ISimpleHelloWorldWcfService");
                Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName), "Received initial ISimpleHelloWorldWcfService client info event");
            }
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestConnectionFailure(ConnectionMode mode)
        {
            Connect(mode, null);

            _netTcpConnectedEvent.Wait(MediumWait);
            _basicHttpConnectedEvent.Wait(ShortWait);

            Assert.IsFalse(_netTcpConnectedEvent.IsSet, "ClientConnected received for IHelloWorldWcfService");
            Assert.IsTrue(_basicHttpConnectedEvent.IsSet, "No ClientConnected received for ISimpleHelloWorldWcfService");

            Assert.NotNull(_basicHttpService, "Didn't get SimpleHelloWorld service");
            Assert.AreEqual(ConnectionState.Success, _basicHttpState, "SimpleHelloWorld state");

            Assert.Null(_netTcpService, "Got HelloWorld service");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries == 1), "Received first failed IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries == 2), "Received second IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.FailedTry && i.Tries == 3), "Received third IHelloWorldWcfService client info event");

                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final ISimpleHelloWorldWcfService client info event");
            }
        }

        [TestCase(ConnectionMode.New, SimpleHelloWorldWcfService.MinClientVersion, "9.9.9.9")]
        [TestCase(ConnectionMode.New, "1.0.0.0", SimpleHelloWorldWcfService.ServerVersion)]
        [TestCase(ConnectionMode.Legacy, SimpleHelloWorldWcfService.MinClientVersion, "9.9.9.9")]
        [TestCase(ConnectionMode.Legacy, "1.0.0.0", SimpleHelloWorldWcfService.ServerVersion)]
        public void TestVersionMismatch(ConnectionMode mode, string clientVersion, string serverVersion)
        {
            Connect(mode, clientVersion, serverVersion);

            _basicHttpConnectedEvent.Wait(MediumWait);
            _netTcpConnectedEvent.Wait(ShortWait);

            Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");
            Assert.IsFalse(_basicHttpConnectedEvent.IsSet, "ClientConnected received for ISimpleHelloWorldWcfService");

            Assert.Null(_basicHttpService, "Got SimpleHelloWorld service");
            Assert.AreEqual(ConnectionState.VersionMissmatch, _basicHttpState, "SimpleHelloWorld state");

            Assert.NotNull(_netTcpService, "Didn't get HelloWorld service");
            Assert.AreEqual(ConnectionState.Success, _netTcpState, "HelloWorld state");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate IHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received final IHelloWorldWcfService client info event");

                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 0), "Received initial ISimpleHelloWorldWcfService client info event");
                Assert.Null(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.New && i.Tries == 1), "Received intermediate ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.VersionMissmatch && i.Tries == 1), "Received first failed ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.VersionMissmatch && i.Tries == 2), "Received second ISimpleHelloWorldWcfService client info event");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == BasicHttpServiceName && i.State == ConnectionState.VersionMissmatch && i.Tries == 3), "Received third ISimpleHelloWorldWcfService client info event");
            }
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestBasicHttpCall(ConnectionMode mode)
        {
            TestConnect(mode);

            string hello = _basicHttpService.Hello(null);
            Assert.AreEqual("Hello world!", hello, "Unexpected result with null argument");

            hello = _basicHttpService.Hello("Phoenix");
            Assert.AreEqual("Hello Phoenix!", hello, "Unexpected result with argument");
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestBasicHttpException(ConnectionMode mode)
        {
            TestConnect(mode);

            Clear(false);

            try
            {
                _basicHttpService.Throw(null);
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(FaultException<ExceptionDetail>), e.GetType(), "Unexpected exception: {0}", e);
            }

            string hello = _basicHttpService.Hello(null);
            Assert.AreEqual("Hello world!", hello, "Unexpected result with null argument");
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestNetTcpCall(ConnectionMode mode)
        {
            TestConnect(mode);

            string hello = _netTcpService.Hello(null);
            Assert.AreEqual("Hello world!", hello, "Unexpected result with null argument");

            hello = _netTcpService.Hello("Phoenix");
            Assert.AreEqual("Hello Phoenix!", hello, "Unexpected result with argument");
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestNetTcpException(ConnectionMode mode)
        {
            TestConnect(mode);

            Clear(false);

            try
            {
                _netTcpService.Throw(null);
            }
            catch (Exception e)
            {
                Assert.AreEqual(typeof(FaultException<ExceptionDetail>), e.GetType(), "Unexpected exception: {0}", e);
            }

            _netTcpDisconnectedEvent.Wait(LongWait);
            _netTcpConnectedEvent.Wait(LongWait);

            Assert.IsTrue(_netTcpDisconnectedEvent.IsSet, "No ClientDisconnected received for IHelloWorldWcfService");
            Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.ConnectionLost), "Received IHelloWorldWcfService client info event 'ConnectionLost'");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received IHelloWorldWcfService client info event 'Success'");
            }

            string hello = _netTcpService.Hello(null);
            Assert.AreEqual("Hello world!", hello, "Unexpected result with null argument");
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestNetTcpCallback(ConnectionMode mode)
        {
            TestConnect(mode);

            _netTcpService.Subscribe(_clientFactory.ClientId);

            _netTcpService.TriggerHelloCallback(null);
            NetTcpCallbackReceived.Wait(MediumWait);

            Assert.IsTrue(NetTcpCallbackReceived.IsSet, "No Callback received for IHelloWorldWcfService");
            Assert.AreEqual("Hello world!", _receivedMessage, "Unexpected result with null argument");

            NetTcpCallbackReceived.Reset();

            _netTcpService.TriggerHelloCallback("Phoenix");
            NetTcpCallbackReceived.Wait(MediumWait);

            Assert.IsTrue(NetTcpCallbackReceived.IsSet, "No Callback received for IHelloWorldWcfService");
            Assert.AreEqual("Hello Phoenix!", _receivedMessage, "Unexpected result with argument");
        }

        [TestCase(ConnectionMode.New)]
        [TestCase(ConnectionMode.Legacy)]
        public void TestNetTcpCallbackException(ConnectionMode mode)
        {
            TestConnect(mode);

            Clear(false);

            _netTcpService.Subscribe(_clientFactory.ClientId);

            _netTcpService.TriggerThrowCallback(null);

            NetTcpThrowCallbackReceived.Wait(MediumWait);

            Assert.IsTrue(NetTcpThrowCallbackReceived.IsSet, "No Callback received for IHelloWorldWcfService");

            _netTcpDisconnectedEvent.Wait(LongWait);
            _netTcpConnectedEvent.Wait(LongWait);

            Assert.IsTrue(_netTcpDisconnectedEvent.IsSet, "No ClientDisconnected received for IHelloWorldWcfService");
            Assert.IsTrue(_netTcpConnectedEvent.IsSet, "No ClientConnected received for IHelloWorldWcfService");

            lock (_receivedClientInfos)
            {
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.ConnectionLost), "Received IHelloWorldWcfService client info event 'ConnectionLost'");
                Assert.NotNull(_receivedClientInfos.FirstOrDefault(i => i.Service == NetTcpServiceName && i.State == ConnectionState.Success && i.Tries == 1), "Received IHelloWorldWcfService client info event 'Success'");
            }

            string hello = _netTcpService.Hello(null);
            Assert.AreEqual("Hello world!", hello, "Unexpected result with null argument");

            //bool result = _hogController.WaitForService("TestModule", ServerModuleState.Warning, 5);
            //Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Warning' ");

            //_hogController.StopService("TestModule");
            //result = _hogController.WaitForService("TestModule", ServerModuleState.Stopped, 5);
            //Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", "TestModule");
        }

        [Test]
        public void TestChangeBinding()
        {
            SetBindingType(BindingType.NetTcp);

            TestConnect(ConnectionMode.New);
        }

        [Test]
        public void TestServiceClose()
        {
            TestConnect(ConnectionMode.New);

            Clear(false);

            _netTcpService.Subscribe(_clientFactory.ClientId);

            _netTcpService.DeferredDisconnect(1000);

            Assert.IsTrue(_netTcpDisconnectedEvent.Wait(MediumWait), "Service did not send disconnect");
        }

        private void SetBindingType(BindingType binding)
        {
            Config config = _hogController.GetConfig("DependentTestModule");

            Entry connectorConfig = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "SimpleHelloWorldWcfConnector");
            Assert.NotNull(connectorConfig, "Can't get config entry 'SimpleHelloWorldWcfConnector'");

            Entry hostConfig = connectorConfig.SubEntries.FirstOrDefault(e => e.Identifier == "ConnectorHost");
            Assert.NotNull(hostConfig, "Can't get config entry 'SimpleHelloWorldWcfConnector.ConnectorHost'");

            Entry bindingType = hostConfig.SubEntries.FirstOrDefault(e => e.Identifier == "BindingType");
            Assert.NotNull(bindingType, "Can't get config entry 'SimpleHelloWorldWcfConnector.ConnectorHost.BindingType'");

            if (bindingType.Value.Current != binding.ToString())
            {
                bindingType.Value.Current = binding.ToString();

                _hogController.SetConfig(config, "DependentTestModule");

                _hogController.StopService("DependentTestModule");
                bool result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Stopped, 5);
                Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", "DependentTestModule");

                _hogController.StartService("DependentTestModule");
                result = _hogController.WaitForService("DependentTestModule", ServerModuleState.Running, 5);
                Assert.IsTrue(result, "Service 'DependentTestModule' did not reach state 'Running'");
            }
        }

        private void Connect(ConnectionMode mode)
        {
            Connect(mode, _netTcpCallback);
        }

        private void Connect(ConnectionMode mode, IHelloWorldWcfServiceCallback netTcpCallback)
        {
            Connect(mode, netTcpCallback, SimpleHelloWorldWcfService.MinClientVersion, SimpleHelloWorldWcfService.ServerVersion);
        }

        private void Connect(ConnectionMode mode, string clientVersion, string minServerVersion)
        {
            Connect(mode, _netTcpCallback, clientVersion, minServerVersion);
        }

        private void Connect(ConnectionMode mode, IHelloWorldWcfServiceCallback netTcpCallback, string clientVersion, string minServerVersion)
        {
            switch (mode)
            {
                case ConnectionMode.New:
                    _clientFactory.Create<HelloWorldWcfServiceClient, IHelloWorldWcfService>(HelloWorldWcfService.MinClientVersion, HelloWorldWcfService.ServerVersion, netTcpCallback, NetTcpCallback);
                    _clientFactory.Create<SimpleHelloWorldWcfServiceClient, ISimpleHelloWorldWcfService>(clientVersion, minServerVersion, BasicHttpCallback);
                    break;

                case ConnectionMode.Legacy:
                    ConnectLegacy(netTcpCallback, null, clientVersion, minServerVersion);
                    break;

                case ConnectionMode.LegacyWithHost:
                    ConnectLegacy(netTcpCallback, "localHost", clientVersion, minServerVersion);
                    break;

                default:
                    Assert.Fail("Unknonw connection mode '{0}'", mode);
                    break;
            }
        }

        private void ConnectLegacy(IHelloWorldWcfServiceCallback netTcpCallback, string host, string clientVersion, string minServerVersion)
        {
            ClientConfig netTcpClientConfig = new ClientConfig
            {
                BindingType = BindingType.NetTcp,
                Endpoint = NetTcpEndpointName,
                Host = host,
                Port = _hogController.NetTcpPort,
                ClientVersion = HelloWorldWcfService.MinClientVersion,
                MinServerVersion = HelloWorldWcfService.ServerVersion
            };

            ClientConfig basicHttpClientConfig = new ClientConfig
            {
                BindingType = BindingType.BasicHttp,
                Endpoint = BasicHttpEndpointName,
                Host = host,
                Port = _hogController.HttpPort,
                ClientVersion = clientVersion,
                MinServerVersion = minServerVersion
            };

            _clientFactory.Create<HelloWorldWcfServiceClient, IHelloWorldWcfService>(netTcpClientConfig, netTcpCallback, NetTcpCallback);
            _clientFactory.Create<SimpleHelloWorldWcfServiceClient, ISimpleHelloWorldWcfService>(basicHttpClientConfig, BasicHttpCallback);
        }

        private void Clear(bool full)
        {
            lock (_receivedClientInfos)
            {
                _receivedClientInfos.Clear();
            }

            _netTcpConnectedEvent.Reset();
            _basicHttpConnectedEvent.Reset();
            _netTcpDisconnectedEvent.Reset();
            _basicHttpDisconnectedEvent.Reset();
            _clientInfoChangedEvent.Reset();
            NetTcpCallbackReceived.Reset();
            NetTcpThrowCallbackReceived.Reset();

            if (full)
            {
                _basicHttpService = null;
                _netTcpService = null;
            }
        }

        private void NetTcpCallback(ConnectionState state, HelloWorldWcfServiceClient service)
        {
            _netTcpState = state;
            _netTcpService = service;
        }

        private void BasicHttpCallback(ConnectionState state, SimpleHelloWorldWcfServiceClient service)
        {
            _basicHttpState = state;
            _basicHttpService = service;
        }

        public class HelloWorldWcfServiceCallback : IHelloWorldWcfServiceCallback
        {
            public void HelloCallback(string message)
            {
                _receivedMessage = message;
                NetTcpCallbackReceived.Set();
            }

            public string ThrowCallback(string message)
            {
                NetTcpThrowCallbackReceived.Set();

                // The exception here is on purpose!
                throw new NotImplementedException();
            }
        }
    }
}
