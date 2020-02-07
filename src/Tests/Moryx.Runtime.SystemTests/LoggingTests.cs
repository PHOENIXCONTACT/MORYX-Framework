// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Moryx.Communication;
using Moryx.Runtime.Kernel;
using Moryx.Runtime.Maintenance.Logging;
using Moryx.Runtime.Maintenance.Modules;
using Moryx.Runtime.Modules;
using Moryx.Serialization;
using Moryx.TestModule;
using Moryx.TestTools.SystemTest;
using Moryx.Tools.Wcf;
using NUnit.Framework;
using LogLevel = Moryx.Logging.LogLevel;

namespace Moryx.Runtime.SystemTests
{
    /// <summary>
    /// Theses tests shall check the logger functionality
    /// </summary>
    [TestFixture]
    public class LoggingTests : IDisposable
    {
        private const int WaitTime = 3000;
        private const int ServerSleepTime = 100;
        private const int ClientSleepTime = 100;

        private HeartOfGoldController _hogController;
        private RuntimeConfigManager  _configManager;
        private int _loggerId;
        private readonly ManualResetEvent _logMessageReceived = new ManualResetEvent(false);
        private LogLevel _receivedLevel;
        private string _receivedMessage;
        private LoggerModel[] _pluginLogger;
        private LoggerModel _testModuleLogger;

        [OneTimeSetUp]
        public void TestFixtureSetUp()
        {
            Directory.CreateDirectory(HogHelper.ConfigDir);

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
                Config = new PortConfig(),

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
            Directory.Delete(HogHelper.ConfigDir, true);

            if (_hogController.Process != null && !_hogController.Process.HasExited)
            {
                Console.WriteLine("Killing HeartOfGold");
                _hogController.Process.Kill();

                Thread.Sleep(1000);

                Assert.IsTrue(_hogController.Process.HasExited, "Can't kill HeartOfGold.");
            }
        }

        [TearDown]
        public void Cleanup()
        {
            _hogController.RemoveRemoteLogAppender("TestModule", _loggerId);
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
        [TestCase(LogLevel.Error, LogLevel.Fatal, false)]
        public void BasicLoggingTest1(LogLevel senderLevel, LogLevel receiverLevel, bool expectedResult)
        {
            BasicLoggingTest(senderLevel, LogLevel.Trace, receiverLevel, expectedResult);
        }

        [TestCase(LogLevel.Fatal, LogLevel.Fatal, true)]
        [TestCase(LogLevel.Error, LogLevel.Fatal, false)]
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

            Entry logLevelEntry = config.Root.SubEntries.FirstOrDefault(e => e.Identifier == "LogLevel");
            Assert.IsNotNull(logLevelEntry, "Can't get property 'LogLevel' from config.");
            logLevelEntry.Value.Current = senderLevel.ToString();
            _hogController.SetConfig(config, "TestModule");

            Thread.Sleep(ClientSleepTime);

            _hogController.ReincarnateServiceAsync("TestModule");
            _hogController.WaitForService("TestModule", ServerModuleState.Running, 10);

            var received = _logMessageReceived.WaitOne(WaitTime);

            Assert.AreEqual(expectedResult, received, "Expected log message received?");

            if (received)
            {
                Assert.AreEqual(senderLevel, _receivedLevel, _receivedMessage);
            }
        }

        private void HandleLogMessages(object sender, HeartOfGoldController.LoggerEventArgs args)
        {
            foreach (var message in args.Messages)
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
