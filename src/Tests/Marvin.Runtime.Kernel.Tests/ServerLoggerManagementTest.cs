using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Marvin.Logging;
using NUnit.Framework;

namespace Marvin.Runtime.Kernel.Tests
{
    /// <summary>
    /// Tests for the server logger management
    /// </summary>
    [TestFixture]
    public class ServerLoggerManagementTest
    {
        private string _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        private RuntimeConfigManager _configManager;
        private IServerLoggerManagement _management;
        private ILoggingHost _host;

        /// <summary>
        /// Initializes the test.
        /// </summary>
        [OneTimeSetUp]
        public void Init()
        {
            Directory.CreateDirectory(_tempDirectory);

            _configManager = new RuntimeConfigManager()
            {
                ConfigDirectory = _tempDirectory
            };

            var config = new LoggingConfig()
            {
                LoggerConfigs = new List<ModuleLoggerConfig>()
                {
                    new ModuleLoggerConfig() {LoggerName = "Test"}
                }
            };

            _configManager.SaveConfiguration(config);
            _management = new ServerLoggerManagement()
            {
                ConfigManager = _configManager
            };

            _host = new ModuleManager()
            {
                ConfigManager = _configManager,
                LoggerManagement = _management
            };
            _management.ActivateLogging(_host);
        }

        /// <summary>
        /// Check if it is possible to change the loglevel of a logger.
        /// </summary>
        /// <param name="initalLevel">The inital level.</param>
        /// <param name="newLevel">The new level.</param>
        [TestCase(LogLevel.Debug, LogLevel.Fatal, Description = "Change logger to fatal")]
        [TestCase(LogLevel.Error, LogLevel.Debug, Description = "Change logger to debug")]
        [TestCase(LogLevel.Info, LogLevel.Error, Description = "Change logger to error")]
        [TestCase(LogLevel.Warning, LogLevel.Info, Description = "Change logger to info")]
        [TestCase(LogLevel.Trace, LogLevel.Warning, Description = "Change logger to warning")]
        [TestCase(LogLevel.Fatal, LogLevel.Trace, Description = "Change logger to trace")]
        public void SetLevelTest(LogLevel initalLevel, LogLevel newLevel)
        {
            // get a logger
            IModuleLogger logger = _host.Logger;
            // change its loglevel
            _management.SetLevel(logger, initalLevel);
            
            Assert.AreEqual(initalLevel, logger.ActiveLevel, "The logger is not in the inital LogLevel!");

            // change its loglevel
            _management.SetLevel(logger, newLevel);

            Assert.AreEqual(newLevel, logger.ActiveLevel, "The LogLevel did not change!");
        }

        /// <summary>
        /// Check if it is possible to change the loglevel of a logger by the loggers name.
        /// </summary>
        [TestCase(LogLevel.Debug, LogLevel.Fatal, Description = "Change logger to fatal")]
        [TestCase(LogLevel.Error, LogLevel.Debug, Description = "Change logger to debug")]
        [TestCase(LogLevel.Info, LogLevel.Error, Description = "Change logger to error")]
        [TestCase(LogLevel.Warning, LogLevel.Info, Description = "Change logger to info")]
        [TestCase(LogLevel.Trace, LogLevel.Warning, Description = "Change logger to warning")]
        [TestCase(LogLevel.Fatal, LogLevel.Trace, Description = "Change logger to trace")]
        public void SetLevelByNameTest(LogLevel initalLevel, LogLevel newLevel)
        {
            // get a logger
            IModuleLogger logger = _host.Logger;
            // change its loglevel
            _management.SetLevel(logger.Name, initalLevel);

            Assert.AreEqual(initalLevel, logger.ActiveLevel, "The logger is not in the inital LogLevel!");

            // set the new loglevel
            _management.SetLevel(logger.Name, newLevel);

            Assert.True(logger.ActiveLevel == newLevel, "The LogLevel did not change!");
        }

        /// <summary>
        /// Check if it is posible to activate and disable logging
        /// </summary>
        [Test]
        public void ToggelLoggingStateTest()
        {
            var host = new ModuleManager()
            {
                ConfigManager = _configManager,
                LoggerManagement = _management
            };

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            ILogMessage logMessage = null;

            // at beginning the logger is null until it will be activated!
            Assert.Null(host.Logger, "The logger is not null!");
            // activate logging
            _management.ActivateLogging(host);
            // logger has been set
            Assert.NotNull(host.Logger, "The logger has not been set!");
            _management.SetLevel(host.Logger, LogLevel.Trace);

            var loggerInstance = host.Logger;

            // get messages from the logger
            _management.AppendListenerToStream(delegate(ILogMessage message)
            {
                logMessage = message;
                manualResetEvent.Set();
            });
            
            // Wait for the management to be ready.
            Thread.Sleep(10);

            // create a logmessage
            host.Logger.Log(LogLevel.Debug, "Testing sucks!");

            // wait a short time or until the message has been received.
            manualResetEvent.WaitOne(10000); // large timer for debug

            Assert.NotNull(logMessage, "The logmessage has not been forwarded!");
            Assert.AreEqual(logMessage.Message, "Testing sucks!", "The message is wrong!");

            // prepare disable test
            manualResetEvent.Reset();
            logMessage = null;
            
            // disable logging
            _management.DeactivateLogging(host);

            // the logger should not change to null!
            Assert.NotNull(host.Logger, "The logger changend to null!");
            // log another message
            host.Logger.Log(LogLevel.Debug, "Testing is useful!");

            // wait a time; now the message should not be forwarded!
            manualResetEvent.WaitOne(1000); // small timer because it normaly must exceed to proceed
            // fail if we got a message 
            Assert.Null(logMessage, "Logging has not been deactivated!");


            //prepare reactivation test
            manualResetEvent.Reset();
            logMessage = null;
            
            // reactivate
            _management.ActivateLogging(host);

            // check if the logger instance changed or not
            var loggerInstance2 = host.Logger;
            Assert.AreEqual(loggerInstance, loggerInstance2, "After reactivation we got a new logger instance!");

            // log another message
            host.Logger.Log(LogLevel.Debug, "Testing is very useful!");
            // wait for the logmessage
            manualResetEvent.WaitOne(10000); // large timer for debug
           
            Assert.NotNull(logMessage, "After reactivation the log message has not been forwarded!");
            Assert.AreEqual(logMessage.Message, "Testing is very useful!", "The message is wrong!");
        }

        /// <summary>
        /// Check loglevel filter
        /// </summary>
        /// <param name="minLevel">The minimum level.</param>
        /// <param name="messageLevel">The message level.</param>
        /// <param name="messageExpected">if set to <c>true</c> [message expected].</param>
        [TestCase(LogLevel.Debug  , LogLevel.Debug  , true)]
        [TestCase(LogLevel.Debug  , LogLevel.Error  , true)]
        [TestCase(LogLevel.Debug  , LogLevel.Fatal  , true)]
        [TestCase(LogLevel.Debug  , LogLevel.Info   , true)]
        [TestCase(LogLevel.Debug  , LogLevel.Trace  , false)]
        [TestCase(LogLevel.Debug  , LogLevel.Warning, true)]
        [TestCase(LogLevel.Error  , LogLevel.Debug  , false)]
        [TestCase(LogLevel.Error  , LogLevel.Error  , true)]
        [TestCase(LogLevel.Error  , LogLevel.Fatal  , true)]
        [TestCase(LogLevel.Error  , LogLevel.Info   , false)]
        [TestCase(LogLevel.Error  , LogLevel.Trace  , false)]
        [TestCase(LogLevel.Error  , LogLevel.Warning, false)]
        [TestCase(LogLevel.Fatal  , LogLevel.Debug  , false)]
        [TestCase(LogLevel.Fatal  , LogLevel.Error  , false)]
        [TestCase(LogLevel.Fatal  , LogLevel.Fatal  , true)]
        [TestCase(LogLevel.Fatal  , LogLevel.Info   , false)]
        [TestCase(LogLevel.Fatal  , LogLevel.Trace  , false)]
        [TestCase(LogLevel.Fatal  , LogLevel.Warning, false)]
        [TestCase(LogLevel.Info   , LogLevel.Debug  , false)]
        [TestCase(LogLevel.Info   , LogLevel.Error  , true)]
        [TestCase(LogLevel.Info   , LogLevel.Fatal  , true)]
        [TestCase(LogLevel.Info   , LogLevel.Info   , true)]
        [TestCase(LogLevel.Info   , LogLevel.Trace  , false)]
        [TestCase(LogLevel.Info   , LogLevel.Warning, true)]
        [TestCase(LogLevel.Trace  , LogLevel.Debug  , true)]
        [TestCase(LogLevel.Trace  , LogLevel.Error  , true)]
        [TestCase(LogLevel.Trace  , LogLevel.Fatal  , true)]
        [TestCase(LogLevel.Trace  , LogLevel.Info   , true)]
        [TestCase(LogLevel.Trace  , LogLevel.Trace  , true)]
        [TestCase(LogLevel.Trace  , LogLevel.Warning, true)]
        [TestCase(LogLevel.Warning, LogLevel.Debug  , false)]
        [TestCase(LogLevel.Warning, LogLevel.Error  , true)]
        [TestCase(LogLevel.Warning, LogLevel.Fatal  , true)]
        [TestCase(LogLevel.Warning, LogLevel.Info   , false)]
        [TestCase(LogLevel.Warning, LogLevel.Trace  , false)]
        [TestCase(LogLevel.Warning, LogLevel.Warning, true)]
        public void AppendListender(LogLevel minLevel, LogLevel messageLevel, bool messageExpected)
        {
            var host = new ModuleManager()
            {
                ConfigManager = _configManager,
                LoggerManagement = _management
            };

            ManualResetEvent manualResetEvent = new ManualResetEvent(false);
            ILogMessage logMessage = null;
            //activate logging
            _management.ActivateLogging(host);

            Assert.NotNull(host.Logger, "The logger has not been set!");

            _management.AppendListenerToStream(delegate(ILogMessage message)
            {
                logMessage = message;
                manualResetEvent.Set();
            }, minLevel);

            // add logentry
            host.Logger.Log(messageLevel, "Testing rocks!");

            // wait to receive the log
            manualResetEvent.WaitOne(1000); // large timer for debug

            // check if it works the correct way
            if (messageExpected)
            {
                Assert.NotNull(logMessage, "The logmessage has not been forwarded, but it should!");
                Assert.True(logMessage.Message == "Testing rocks!", "The message is wrong!");
            }
            else
                Assert.Null(logMessage, "The logmessage has been forwarded, but it should not!");
        }

        /// <summary>
        /// Shuts down the test.
        /// </summary>
        [OneTimeTearDown]
        public void TearDown()
        {
            if (Directory.Exists(_tempDirectory))
                Directory.Delete(_tempDirectory, true);
        }
    }
}
