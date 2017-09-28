using System;
using System.Collections.Generic;
using System.Linq;
using Marvin.Logging;
using NUnit.Framework;

namespace Marvin.PlatformTools.Tests.Logging
{
    [TestFixture]
    public class LoggerManagementTests
    {
        private TestLoggerManagement _loggerManagement;
        private ModuleMock _module;

        [SetUp]
        public void Setup()
        {
            _loggerManagement = new TestLoggerManagement();
            _module = new ModuleMock();
        }

        [TearDown]
        public void TearDown()
        {
            _loggerManagement.Dispose();
        }

        [Test]
        public void TestActivateLogging()
        {
            _loggerManagement.ActivateLogging(_module);

            Assert.NotNull(_module.Logger);
            Assert.AreEqual(typeof(Logger), _module.Logger.GetType());
            Assert.AreEqual(_module.Name, _module.Logger.Name);
            Assert.AreEqual(LogLevel.Info, _module.Logger.ActiveLevel);
        }

        [Test]
        public void TestDeactivateLogging()
        {
            _loggerManagement.DeactivateLogging(_module);

            Assert.NotNull(_module.Logger);
            Assert.AreEqual(typeof(IdleLogger), _module.Logger.GetType());
        }

        [Test]
        public void TestGetChild()
        {
            TestActivateLogging();

            Logger logger = (Logger)_module.Logger;

            IModuleLogger clone = logger.GetChild(null, _module.GetType());

            IModuleLogger childA = logger.GetChild(null);
            IModuleLogger childB = logger.GetChild(null, GetType());
            IModuleLogger childC = logger.GetChild(null, typeof(Object));
            IModuleLogger childD = logger.GetChild(null, typeof(Object));

            Assert.IsTrue(Object.ReferenceEquals(logger, clone), "logger, clone");

            Assert.IsFalse(Object.ReferenceEquals(logger, childA), "logger, childA");
            Assert.IsTrue(Object.ReferenceEquals(childA, childB), "childA, childB");
            Assert.IsFalse(Object.ReferenceEquals(logger, childC), "logger, childC");
            Assert.IsFalse(Object.ReferenceEquals(childA, childC), "childA, childC");
            Assert.IsTrue(Object.ReferenceEquals(childC, childD), "childC, childD");

            IModuleLogger child1 = logger.GetChild("MyChildLogger");
            IModuleLogger child2 = logger.GetChild("MyChildLogger", GetType());
            IModuleLogger child3 = logger.GetChild("MyChildLogger", typeof(Object));
            IModuleLogger child4 = logger.GetChild("MyChildLogger", typeof(Object));
            IModuleLogger child5 = logger.GetChild("MyOtherChildLogger");
            IModuleLogger child6 = logger.GetChild("MyOtherChildLogger", GetType());
            IModuleLogger child7 = logger.GetChild("MyOtherChildLogger", typeof(Object));
            IModuleLogger child8 = logger.GetChild("MyOtherChildLogger", typeof(Object));

            Assert.IsTrue(Object.ReferenceEquals(child1, child2), "child1, child2");
            Assert.IsFalse(Object.ReferenceEquals(child2, child3), "child2, child3");
            Assert.IsTrue(Object.ReferenceEquals(child3, child4), "child3, child4");
            Assert.IsTrue(Object.ReferenceEquals(child5, child6), "child5, child6");
            Assert.IsFalse(Object.ReferenceEquals(child6, child7), "child6, child7");
            Assert.IsTrue(Object.ReferenceEquals(child7, child8), "child7, child8");

            Assert.IsFalse(Object.ReferenceEquals(child1, child5), "child1, child5");
        }

        [Test]
        public void TestSetLevelByLogger()
        {
            TestActivateLogging();

            Logger logger = (Logger) _module.Logger;

            IModuleLogger child1 = logger.GetChild("MyChildLogger");
            IModuleLogger child2 = logger.GetChild("MyChildLogger", typeof(Object));
            IModuleLogger child3 = logger.GetChild("MyOtherChildLogger");
            IModuleLogger child4 = logger.GetChild("MyOtherChildLogger", typeof(Object));

            Assert.AreEqual(LogLevel.Info, child1.ActiveLevel, "Initial child1 log level");
            Assert.AreEqual(LogLevel.Info, child2.ActiveLevel, "Initial child2 log level");
            Assert.AreEqual(LogLevel.Info, child3.ActiveLevel, "Initial child3 log level");
            Assert.AreEqual(LogLevel.Info, child4.ActiveLevel, "Initial child4 log level");

            _loggerManagement.SetLevel(_module.Logger, LogLevel.Debug);

            Assert.AreEqual(LogLevel.Debug, _module.Logger.ActiveLevel, "New log level");
            Assert.AreEqual(LogLevel.Debug, child1.ActiveLevel, "New child1 log level");
            Assert.AreEqual(LogLevel.Debug, child2.ActiveLevel, "New child2 log level");
            Assert.AreEqual(LogLevel.Debug, child3.ActiveLevel, "New child3 log level");
            Assert.AreEqual(LogLevel.Debug, child4.ActiveLevel, "New child4 log level");

            _loggerManagement.SetLevel(child3, LogLevel.Error);

            Assert.AreEqual(LogLevel.Debug, _module.Logger.ActiveLevel, "Third log level");
            Assert.AreEqual(LogLevel.Debug, child1.ActiveLevel, "Third child1 log level");
            Assert.AreEqual(LogLevel.Debug, child2.ActiveLevel, "Third child2 log level");
            Assert.AreEqual(LogLevel.Error, child3.ActiveLevel, "Third child3 log level");
            Assert.AreEqual(LogLevel.Error, child4.ActiveLevel, "Third child4 log level");

        }

        [Test]
        public void TestSetLevelByName()
        {
            TestActivateLogging();

            Logger logger = (Logger)_module.Logger;

            IModuleLogger child1 = logger.GetChild("MyChildLogger");
            IModuleLogger child2 = logger.GetChild("MyChildLogger", typeof(Object));
            IModuleLogger child3 = logger.GetChild("MyOtherChildLogger");
            IModuleLogger child4 = logger.GetChild("MyOtherChildLogger", typeof(Object));

            Assert.AreEqual(LogLevel.Info, child1.ActiveLevel, "Initial child1 log level");
            Assert.AreEqual(LogLevel.Info, child2.ActiveLevel, "Initial child2 log level");
            Assert.AreEqual(LogLevel.Info, child3.ActiveLevel, "Initial child3 log level");
            Assert.AreEqual(LogLevel.Info, child4.ActiveLevel, "Initial child4 log level");

            _loggerManagement.SetLevel(_module.Logger.Name, LogLevel.Debug);

            Assert.AreEqual(LogLevel.Debug, _module.Logger.ActiveLevel, "New log level");
            Assert.AreEqual(LogLevel.Debug, child1.ActiveLevel, "New child1 log level");
            Assert.AreEqual(LogLevel.Debug, child2.ActiveLevel, "New child2 log level");
            Assert.AreEqual(LogLevel.Debug, child3.ActiveLevel, "New child3 log level");
            Assert.AreEqual(LogLevel.Debug, child4.ActiveLevel, "New child4 log level");

            _loggerManagement.SetLevel(child3.Name, LogLevel.Error);

            Assert.AreEqual(LogLevel.Debug, _module.Logger.ActiveLevel, "Third log level");
            Assert.AreEqual(LogLevel.Debug, child1.ActiveLevel, "Third child1 log level");
            Assert.AreEqual(LogLevel.Debug, child2.ActiveLevel, "Third child2 log level");
            Assert.AreEqual(LogLevel.Error, child3.ActiveLevel, "Third child3 log level");
            Assert.AreEqual(LogLevel.Error, child4.ActiveLevel, "Third child4 log level");
        }

        [Test]
        public void TestGetEnumerator()
        {
            TestActivateLogging();

            Logger logger = (Logger)_module.Logger;

            IModuleLogger child1 = logger.GetChild("MyChildLogger");
            IModuleLogger child2 = logger.GetChild("MyChildLogger", typeof(Object));
            IModuleLogger child3 = logger.GetChild("MyOtherChildLogger");
            IModuleLogger child4 = logger.GetChild("MyOtherChildLogger", typeof(Object));
            IModuleLogger grandchild1 = child1.GetChild("MyGrandChildLogger", typeof(Object));
            IModuleLogger grandchild2 = child1.GetChild("MyOtherGrandChildLogger", typeof(Object));

            Assert.AreEqual(1, _loggerManagement.Count(), "Number of main loggers");
            Assert.AreEqual(2, logger.Count(), "Number of children");
            Assert.AreEqual(2, child1.Count(), "First number of grandchildren");
            Assert.AreEqual(0, child3.Count(), "Second number of grandchildren");
        }
    }
}