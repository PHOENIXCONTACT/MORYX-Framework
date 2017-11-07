using System;
using System.Linq;
using System.Threading;
using Marvin.Runtime.Kernel.Configuration;
using Marvin.Serialization;
using Marvin.TestModule;
using Marvin.TestTools.SystemTest;
using Marvin.TestTools.SystemTest.Logging;
using Marvin.TestTools.SystemTest.Maintenance;
using Marvin.Tools.Wcf;
using NUnit.Framework;
using LogLevel = Marvin.Logging.LogLevel;

namespace Marvin.Runtime.SystemTests
{
    /// <summary>
    /// Theses tests shall check the logger functionality
    /// </summary>
    [TestFixture]
    public class LoggingTests : IDisposable
    {
        private const int WaitTime = 1000;
        private const int ServerSleepTime = 100;
        private const int ClientSleepTime = 100;

        private HeartOfGoldController _hogController;
        private RuntimeConfigManager  _configManager;
        private int _loggerId;
        private readonly ManualResetEvent _logMessageReceived = new ManualResetEvent(false);
        private LogLevel _receivedLevel;
        private string _receivedMessage;
        private PluginLoggerModel[] _pluginLogger;
        private PluginLoggerModel _testModuleLogger;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            HogHelper.CopyTestModule("Marvin.TestModule.dll");
            HogHelper.DeleteTestModule("Marvin.DependentTestModule.dll");

            _configManager = new RuntimeConfigManager
            {
                ConfigDirectory = HogHelper.ConfigDir
            };

            _hogController = new HeartOfGoldController
            {
                RuntimeDir = HogHelper.RuntimeDir,
                ConfigDir = HogHelper.ConfigDirParam,
                ExecutionTimeout = 1000,
                TimerInterval = 100
            };

            _hogController.LogMessagesReceived += HandleLogMessages;

            ModuleConfig config = new ModuleConfig
            {
                Config = new WcfConfig(),

                SleepTime = ServerSleepTime,
                LogLevel = LogLevel.Info
            };

            _configManager.SaveConfiguration(config);

            Console.WriteLine("Starting HeartOfGold");

            bool started = _hogController.StartHeartOfGold();
            _hogController.CreateClients();

            Assert.IsTrue(started, "Can't start HeartOfGold.");
            Assert.IsFalse(_hogController.Process.HasExited, "HeartOfGold has exited unexpectedly.");

            bool result = _hogController.WaitForService("TestModule", ServerModuleState.Running, 10);
            Assert.IsTrue(result, "Service 'TestModule' did not reach state 'Running'");

            _pluginLogger = _hogController.GetAllPluginLogger();

            _testModuleLogger = _pluginLogger.FirstOrDefault(l => l.Name == "TestModule");
            Assert.NotNull(_testModuleLogger, "Can't get logger configuration for TestModule");
        }

        [OneTimeTearDown]
        public void TestFixtureCleanup()
        {
            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Killing HeartOfGold");
                _hogController.Process.Kill();

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
            }

            HogHelper.DeleteTestModule("Marvin.TestModule.dll");
        }

        [TearDown]
        public void Cleanup()
        {
            _hogController.RemoveRemoteLogAppender(_loggerId);
        }

        public void Dispose()
        {
            if (_hogController != null)
            {
                _hogController.Dispose();
                _hogController = null;
            }
        }

        [TestCase(LogLevel.Fatal, LogLevel.Fatal, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Error, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Warning, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Info, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Debug, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Trace, true)]

        [TestCase(LogLevel.Error, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Error, LogLevel.Error, true)]
        [TestCase(LogLevel.Error, LogLevel.Warning, true)]
        [TestCase(LogLevel.Error, LogLevel.Info, true)]
        [TestCase(LogLevel.Error, LogLevel.Debug, true)]
        [TestCase(LogLevel.Error, LogLevel.Trace, true)]

        [TestCase(LogLevel.Warning, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Warning, LogLevel.Error, false)]
        [TestCase(LogLevel.Warning, LogLevel.Warning, true)]
        [TestCase(LogLevel.Warning, LogLevel.Info, true)]
        [TestCase(LogLevel.Warning, LogLevel.Debug, true)]
        [TestCase(LogLevel.Warning, LogLevel.Trace, true)]

        [TestCase(LogLevel.Info, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Info, LogLevel.Error, false)]
        [TestCase(LogLevel.Info, LogLevel.Warning, false)]
        [TestCase(LogLevel.Info, LogLevel.Info, true)]
        [TestCase(LogLevel.Info, LogLevel.Debug, true)]
        [TestCase(LogLevel.Info, LogLevel.Trace, true)]

        [TestCase(LogLevel.Debug, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Debug, LogLevel.Error, false)]
        [TestCase(LogLevel.Debug, LogLevel.Warning, false)]
        [TestCase(LogLevel.Debug, LogLevel.Info, false)]
        [TestCase(LogLevel.Debug, LogLevel.Debug, true)]
        [TestCase(LogLevel.Debug, LogLevel.Trace, true)]

        [TestCase(LogLevel.Trace, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Trace, LogLevel.Error, false)]
        [TestCase(LogLevel.Trace, LogLevel.Warning, false)]
        [TestCase(LogLevel.Trace, LogLevel.Info, false)]
        [TestCase(LogLevel.Trace, LogLevel.Debug, false)]
        [TestCase(LogLevel.Trace, LogLevel.Trace, true)]
        public void BasicLoggingTest1(LogLevel senderLevel, LogLevel receiverLevel, bool expectedResult)
        {
            BasicLoggingTest(senderLevel, LogLevel.Trace, receiverLevel, expectedResult);
        }

        [TestCase(LogLevel.Fatal, LogLevel.Fatal, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Error, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Warning, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Info, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Debug, true)]
        [TestCase(LogLevel.Fatal, LogLevel.Trace, true)]

        [TestCase(LogLevel.Error, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Error, LogLevel.Error, true)]
        [TestCase(LogLevel.Error, LogLevel.Warning, true)]
        [TestCase(LogLevel.Error, LogLevel.Info, true)]
        [TestCase(LogLevel.Error, LogLevel.Debug, true)]
        [TestCase(LogLevel.Error, LogLevel.Trace, true)]

        [TestCase(LogLevel.Warning, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Warning, LogLevel.Error, false)]
        [TestCase(LogLevel.Warning, LogLevel.Warning, true)]
        [TestCase(LogLevel.Warning, LogLevel.Info, true)]
        [TestCase(LogLevel.Warning, LogLevel.Debug, true)]
        [TestCase(LogLevel.Warning, LogLevel.Trace, true)]

        [TestCase(LogLevel.Info, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Info, LogLevel.Error, false)]
        [TestCase(LogLevel.Info, LogLevel.Warning, false)]
        [TestCase(LogLevel.Info, LogLevel.Info, true)]
        [TestCase(LogLevel.Info, LogLevel.Debug, true)]
        [TestCase(LogLevel.Info, LogLevel.Trace, true)]

        [TestCase(LogLevel.Debug, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Debug, LogLevel.Error, false)]
        [TestCase(LogLevel.Debug, LogLevel.Warning, false)]
        [TestCase(LogLevel.Debug, LogLevel.Info, false)]
        [TestCase(LogLevel.Debug, LogLevel.Debug, true)]
        [TestCase(LogLevel.Debug, LogLevel.Trace, true)]

        [TestCase(LogLevel.Trace, LogLevel.Fatal, false)]
        [TestCase(LogLevel.Trace, LogLevel.Error, false)]
        [TestCase(LogLevel.Trace, LogLevel.Warning, false)]
        [TestCase(LogLevel.Trace, LogLevel.Info, false)]
        [TestCase(LogLevel.Trace, LogLevel.Debug, false)]
        [TestCase(LogLevel.Trace, LogLevel.Trace, true)]
        public void BasicLoggingTest2(LogLevel senderLevel, LogLevel receiverLevel, bool expectedResult)
        {
            BasicLoggingTest(senderLevel, receiverLevel, LogLevel.Trace, expectedResult);
        }

        private void BasicLoggingTest(LogLevel senderLevel, LogLevel loggerLevel, LogLevel appenderLevel, bool expectedResult)
        {
            _hogController.SetLogLevel(_testModuleLogger, loggerLevel);

            Thread.Sleep(ClientSleepTime);

            _pluginLogger = _hogController.GetAllPluginLogger();

            _testModuleLogger = _pluginLogger.FirstOrDefault(l => l.Name == "TestModule");
            Assert.NotNull(_testModuleLogger, "Can't get logger configuration for TestModule");
            // ReSharper disable once PossibleNullReferenceException
            Assert.AreEqual(loggerLevel, _testModuleLogger.ActiveLevel, "Can't set logger configuration for TestModule");

            _logMessageReceived.Reset();

            _loggerId = _hogController.AddRemoteLogAppender("TestModule", appenderLevel);

            Config config = _hogController.GetConfig("TestModule");

            Entry logLevelEntry = config.Entries.FirstOrDefault(e => e.Key.Identifier == "LogLevel");
            Assert.IsNotNull(logLevelEntry, "Can't get property 'LogLevel' from config.");
            logLevelEntry.Value.Current = senderLevel.ToString();
            _hogController.SetConfig(config);

            Thread.Sleep(ClientSleepTime);

            _hogController.ReincarnateServiceAsync("TestModule");
            _hogController.WaitForService("TestModule", ServerModuleState.Running, 10);

            bool received = _logMessageReceived.WaitOne(WaitTime);

            Assert.AreEqual(expectedResult, received, "Expected log message received?");

            if (received)
            {
                Assert.AreEqual(senderLevel, _receivedLevel, _receivedMessage);
            }
        }


        private void HandleLogMessages(object sender, HeartOfGoldController.LoggerEventArgs args)
        {
            foreach (var message in args.Messages.Messages)
            {
                if (!message.Message.StartsWith("Sending log message with level"))
                    continue;

                _receivedLevel = message.LogLevel;
                _receivedMessage = message.Message;

                _logMessageReceived.Set();
            }
        }

    }
}