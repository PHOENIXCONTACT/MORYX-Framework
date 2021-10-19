// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Linq;
using System.Net;
using System.Threading;
using Moryx.Communication;
using Moryx.Communication.Endpoints;
using Moryx.DependentTestModule;
using Moryx.Runtime.Modules;
using Moryx.Runtime.SystemTests;
using Moryx.TestModule;
using Moryx.TestTools.SystemTest;
using NUnit.Framework;
using ModuleController = Moryx.TestModule.ModuleController;

namespace Moryx.Tools.Wcf.SystemTests
{
    [TestFixture]
    public class VersionServiceTests : IDisposable
    {
        private HeartOfGoldController _hogController;
        private IVersionServiceManager _versionService;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 120
            };

            Console.WriteLine("Starting HeartOfGold");

            var started = _hogController.StartApplication();
            _hogController.CreateClients();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            var result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");

            _versionService = new WcfVersionServiceManager(new ProxyConfig(), "localhost", _hogController.HttpPort);

            Assert.NotNull(_versionService, "Can't create VersionServiceClient");
        }

        [SetUp]
        public void SetUp()
        {
            _hogController.StartService(DependentTestModule.ModuleController.ModuleName);
            var result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Running' ", DependentTestModule.ModuleController.ModuleName);
        }

        /// <summary>
        /// Shut down the system test.
        /// </summary>
        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            if (_hogController.Process == null || _hogController.Process.HasExited)
                return;

            Console.WriteLine("Trying to stop HeartOfGold");

            _hogController.StopHeartOfGold(10);

            if (_hogController.Process.HasExited)
                return;

            Console.WriteLine("Killing HeartOfGold");
            _hogController.Process.Kill();

            Thread.Sleep(1000);

            Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
        }

        public void Dispose()
        {
            if (_hogController == null)
                return;

            _hogController.Dispose();
            _hogController = null;
        }

        [TestCase(nameof(ISimpleHelloWorldWcfService), SimpleHelloWorldWcfService.ServerVersion)]
        [TestCase(nameof(IHelloWorldWcfService), HelloWorldWcfService.ServerVersion)]
        public void TestEndpointData(string serviceName, string serverVersion)
        {
            var endpoint = _versionService.ServiceEndpoints(serviceName).FirstOrDefault();

            Assert.NotNull(endpoint, "Endpoint for service {0} not found.", serviceName);

            Assert.AreEqual(serverVersion, endpoint.Version);
        }

        [TestCase(nameof(ISimpleHelloWorldWcfService), ExpectedResult = SimpleHelloWorldWcfService.ServerVersion)]
        [TestCase(nameof(IHelloWorldWcfService), ExpectedResult = HelloWorldWcfService.ServerVersion)]
        public string TestGetServerVersion(string serviceName)
        {
            return _versionService.ServiceEndpoints(serviceName)[0].Version;
        }

        [Test]
        public void TestActiveEndpointsLifeCycle()
        {
            var endpoints = _versionService.ActiveEndpoints();

            var startLength = endpoints.Length;

            Assert.Greater(startLength, 2);

            _hogController.StopService(DependentTestModule.ModuleController.ModuleName);
            var result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", DependentTestModule.ModuleController.ModuleName);

            endpoints = _versionService.ActiveEndpoints();

            Assert.AreEqual(startLength - 1, endpoints.Length, "{0} stopped", DependentTestModule.ModuleController.ModuleName);

            _hogController.StartService(DependentTestModule.ModuleController.ModuleName);
            result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Running' ", DependentTestModule.ModuleController.ModuleName);

            endpoints = _versionService.ActiveEndpoints();

            Assert.AreEqual(startLength, endpoints.Length, "{0} started", DependentTestModule.ModuleController.ModuleName);

            _hogController.StopService(ModuleController.ModuleName);
            result = _hogController.WaitForService(ModuleController.ModuleName, ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", ModuleController.ModuleName);

            endpoints = _versionService.ActiveEndpoints();

            Assert.AreEqual(startLength - 2, endpoints.Length, "{0} stopped", ModuleController.ModuleName);

            _hogController.StartService(ModuleController.ModuleName);
            result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Running' ", ModuleController.ModuleName);

            endpoints = _versionService.ActiveEndpoints();

            Assert.AreEqual(startLength, endpoints.Length, "{0} started", ModuleController.ModuleName);
        }

        [TestCase("IHelloWorldWcfService", HelloWorldWcfService.ServerVersion,  ServiceBindingType.NetTcp, "net.tcp://{HOST}:{PORT}/HelloWorldWcfService/")]
        [TestCase("ISimpleHelloWorldWcfService", SimpleHelloWorldWcfService.ServerVersion, ServiceBindingType.BasicHttp, "http://{HOST}:{PORT}/SimpleHelloWorldWcfService/")]
        public void TestServiceConfig(string service, string serverVersion, ServiceBindingType binding, string url)
        {
            url = url.Replace("{PORT}", binding == ServiceBindingType.NetTcp ? _hogController.NetTcpPort.ToString() : _hogController.HttpPort.ToString())
                .Replace("{HOST}", Dns.GetHostName());

            var serviceConfig = _versionService.ServiceEndpoints(service)[0];
            Assert.NotNull(serviceConfig, "ServiceConfig for service {0} not found.", service);

            Assert.AreEqual(serverVersion, serviceConfig.Version);
            Assert.AreEqual(binding, ((Endpoint)serviceConfig).Binding);
            Assert.AreEqual(url, serviceConfig.Address);
        }

        [Test]
        public void TestServiceConfigLifeCycle()
        {
            var helloServiceName = nameof(IHelloWorldWcfService);
            var simpleServiceName = nameof(ISimpleHelloWorldWcfService);

            Endpoint[] serviceConfigs = GetServiceConfigs();

            Assert.NotNull(serviceConfigs[0], "{0} @ initial", helloServiceName);
            Assert.NotNull(serviceConfigs[1], "{0} @ initial", simpleServiceName);

            _hogController.StopService(DependentTestModule.ModuleController.ModuleName);
            bool result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", DependentTestModule.ModuleController.ModuleName);

            serviceConfigs = GetServiceConfigs();

            Assert.NotNull(serviceConfigs[0], "{0} @ {1} stopped", helloServiceName, DependentTestModule.ModuleController.ModuleName);
            Assert.Null(serviceConfigs[1], "{0} @ {1} stopped", simpleServiceName, DependentTestModule.ModuleController.ModuleName);

            _hogController.StartService(DependentTestModule.ModuleController.ModuleName);
            result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Running' ", DependentTestModule.ModuleController.ModuleName);

            serviceConfigs = GetServiceConfigs();

            Assert.NotNull(serviceConfigs[0], "{0} @ {1} started", helloServiceName, DependentTestModule.ModuleController.ModuleName);
            Assert.NotNull(serviceConfigs[1], "{0} @ {1} started", simpleServiceName, DependentTestModule.ModuleController.ModuleName);

            _hogController.StopService(ModuleController.ModuleName);
            result = _hogController.WaitForService(ModuleController.ModuleName, ServerModuleState.Stopped, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Stopped'", ModuleController.ModuleName);

            serviceConfigs = GetServiceConfigs();

            Assert.Null(serviceConfigs[0], "{0} @ {1} stopped", helloServiceName, ModuleController.ModuleName);
            Assert.Null(serviceConfigs[1], "{0} @ {1} stopped", simpleServiceName, ModuleController.ModuleName);

            _hogController.StartService(ModuleController.ModuleName);
            result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 5);
            Assert.IsTrue(result, "Service '{0}' did not reach state 'Running' ", ModuleController.ModuleName);

            serviceConfigs = GetServiceConfigs();

            Assert.NotNull(serviceConfigs[0], "{0} @ {1} started", helloServiceName, ModuleController.ModuleName);
            Assert.NotNull(serviceConfigs[1], "{0} @ {1} started", simpleServiceName, ModuleController.ModuleName);
        }

        private Endpoint[] GetServiceConfigs()
        {
            var result = new Endpoint[2];

            result[0] = (Endpoint)_versionService.ServiceEndpoints(nameof(IHelloWorldWcfService)).FirstOrDefault();
            result[1] = (Endpoint)_versionService.ServiceEndpoints(nameof(ISimpleHelloWorldWcfService)).FirstOrDefault();

            return result;
        }
    }
}
