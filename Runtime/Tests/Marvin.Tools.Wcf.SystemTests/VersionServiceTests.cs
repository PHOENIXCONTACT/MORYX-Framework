using System;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Marvin.DependentTestModule;
using Marvin.Runtime.Modules;
using Marvin.Runtime.SystemTests;
using Marvin.TestModule;
using Marvin.TestTools.SystemTest;
using NUnit.Framework;
using ModuleController = Marvin.TestModule.ModuleController;

namespace Marvin.Tools.Wcf.SystemTests
{
    [TestFixture]
    public class VersionServiceTests : IDisposable
    {
        private HeartOfGoldController _hogController;
        private IVersionService _versionService;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            HogHelper.CopyTestModules();

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 120
            };

            Console.WriteLine("Starting HeartOfGold");

            bool started = _hogController.StartHeartOfGold();
            _hogController.CreateClients();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            var result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");

            var channelFactory = new ChannelFactory<IVersionService>(HeartOfGoldController.CreateBasicHttpBinding());
            _versionService = channelFactory.CreateChannel(new EndpointAddress("http://localhost/ServiceVersions"));

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

#if ! DEBUG
                HogHelper.DeleteTestModules();
#endif
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

        [TestCase(SimpleHelloWorldWcfService.ServiceName, SimpleHelloWorldWcfService.ServerVersion, SimpleHelloWorldWcfService.MinClientVersion)]
        [TestCase(HelloWorldWcfService.ServiceName, HelloWorldWcfService.ServerVersion, HelloWorldWcfService.MinClientVersion)]
        public void TestEndpointData(string serviceName, string serverVersion, string minClientVersion)
        {
            ServiceEndpoint[] endpoints = _versionService.ActiveEndpoints();

            ServiceEndpoint endpoint = endpoints.FirstOrDefault(e => e.Endpoint == serviceName);

            Assert.NotNull(endpoint, "Endpoint for service {0} not found.", serviceName);

            Assert.AreEqual(serverVersion, endpoint.Version);
            Assert.AreEqual(minClientVersion, endpoint.MinClientVersion);
        }

        [TestCase(SimpleHelloWorldWcfService.ServiceName, ExpectedResult = SimpleHelloWorldWcfService.ServerVersion)]
        [TestCase(HelloWorldWcfService.ServiceName, ExpectedResult = HelloWorldWcfService.ServerVersion)]
        public string TestGetServerVersion(string serviceName)
        {
            return _versionService.GetServerVersion(serviceName);
        }

        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.3.2.1", ExpectedResult = true)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.3.2.2", ExpectedResult = true)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.3.3.0", ExpectedResult = true)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.4.1.0", ExpectedResult = true)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "5.2.1.0", ExpectedResult = true)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.3.2.0", ExpectedResult = false)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.3.1.2", ExpectedResult = false)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "4.2.3.2", ExpectedResult = false)]
        [TestCase(SimpleHelloWorldWcfService.ServiceName, "3.4.3.2", ExpectedResult = false)]
        public bool TestClientSupported(string serviceName, string clientVersion)
        {
            return _versionService.ClientSupported(serviceName, clientVersion);
        }

        [Test]
        public void TestActiveEndpointsLifeCycle()
        {
            ServiceEndpoint[] endpoints = _versionService.ActiveEndpoints();

            int startLength = endpoints.Length;

            Assert.Greater(startLength, 2);

            _hogController.StopService(DependentTestModule.ModuleController.ModuleName);
            bool result = _hogController.WaitForService(DependentTestModule.ModuleController.ModuleName, ServerModuleState.Stopped, 5);
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

        [TestCase("IHelloWorldWcfService", HelloWorldWcfService.ServerVersion, HelloWorldWcfService.MinClientVersion, ServiceBindingType.NetTcp, "net.tcp://localhost:816/HelloWorldWcfService")]
        [TestCase("ISimpleHelloWorldWcfService", SimpleHelloWorldWcfService.ServerVersion, SimpleHelloWorldWcfService.MinClientVersion, ServiceBindingType.BasicHttp, "http://localhost:80/SimpleHelloWorldWcfService")]
        public void TestServiceConfig(string service, string serverVersion, string minClientVersion, ServiceBindingType binding, string url)
        {
            ServiceConfig serviceConfig = _versionService.GetServiceConfiguration(service);

            Assert.NotNull(serviceConfig, "ServiceConfig for service {0} not found.", service);

            Assert.AreEqual(serverVersion, serviceConfig.ServerVersion);
            Assert.AreEqual(minClientVersion, serviceConfig.MinClientVersion);
            Assert.AreEqual(binding, serviceConfig.Binding);
            Assert.AreEqual(url, serviceConfig.ServiceUrl);
        }

        [Test]
        public void TestServiceConfigLifeCycle()
        {
            string helloServiceName = typeof(IHelloWorldWcfService).Name;
            string simpleServiceName = typeof(ISimpleHelloWorldWcfService).Name;

            ServiceConfig[] serviceConfigs = GetServiceConfigs();

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

        private ServiceConfig[] GetServiceConfigs()
        {
            ServiceConfig[] result = new ServiceConfig[2];

            result[0] = _versionService.GetServiceConfiguration(typeof(IHelloWorldWcfService).Name);
            result[1] = _versionService.GetServiceConfiguration(typeof(ISimpleHelloWorldWcfService).Name);

            return result;
        }
    }
}