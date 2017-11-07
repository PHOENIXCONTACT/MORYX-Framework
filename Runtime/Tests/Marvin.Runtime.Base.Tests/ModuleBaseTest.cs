using System.Linq;
using Marvin.Runtime.Base.Tests.Mocks;
using Marvin.Runtime.Base.Tests.Modules;
using Marvin.Runtime.Container;
using NUnit.Framework;

namespace Marvin.Runtime.Base.Tests
{
    [TestFixture]
    public class ModuleBaseTest
    {
        private TestModule _moduleUnderTest;

        [SetUp]
        public void Init()
        {
            _moduleUnderTest = new TestModule
            {
                ConfigManager = new TestConfigManager(),
                LoggerManagement = new TestLoggerMgmt()
            };
        }

        [Test]
        public void StrategiesInConfigFound()
        {
            var casted = (IServerModule) _moduleUnderTest;
            casted.Initialize();

            var containerConfig = ((IContainerHost) _moduleUnderTest).Strategies;

            Assert.GreaterOrEqual(containerConfig.Count, 1, "No strategy found!");
            Assert.IsTrue(containerConfig.ContainsKey(typeof(IStrategy)), "Wrong type!");
            Assert.AreEqual("Test", containerConfig[typeof(IStrategy)], "Wrong implementation instance set!");
        }
    }
}