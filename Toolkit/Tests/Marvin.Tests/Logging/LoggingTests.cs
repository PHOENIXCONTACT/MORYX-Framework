using System;
using System.Linq;
using Marvin.Logging;
using NUnit.Framework;

namespace Marvin.Tests.Logging
{
    [TestFixture]
    public class LoggingTests
    {
        private const string LogMsgFormat = "Hello {0}";
        private const string LogMsgArgument = "World";
        private const string ExceptionMsg = "An exception message";
        private const int EventWaitTime = 50;

        private TestLoggerManagement _loggerManagement;
        private ModuleMock _module;
        private IModuleLogger _logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _loggerManagement = new TestLoggerManagement();
            _module = new ModuleMock();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _loggerManagement.Dispose();
        }

        [SetUp]
        public void Setup()
        {
            _loggerManagement.ClearMessages();
            _loggerManagement.ActivateLogging(_module);
            _loggerManagement.SetLevel(_module.Logger, LogLevel.Info);
            _loggerManagement.MessageReceivedEvent.Reset();

            Assert.NotNull(_module.Logger);
            Assert.AreEqual(typeof(Logger), _module.Logger.GetType());
            Assert.AreEqual(_module.Name, _module.Logger.Name);
            Assert.AreEqual(LogLevel.Info, _module.Logger.ActiveLevel);

            _logger = _module.Logger;
        }

        [TearDown]
        public void TearDown()
        {
            _loggerManagement.DeactivateLogging(_module);

            Assert.NotNull(_module.Logger);
            Assert.AreEqual(typeof(IdleLogger), _module.Logger.GetType());
        }

        [Test]
        public void TestLogEntry()
        {
            DateTime t1 = DateTime.Now;

            _logger.LogEntry(LogLevel.Info, LogMsgFormat, LogMsgArgument);

            _loggerManagement.MessageReceivedEvent.Wait(1000);

            DateTime t2 = DateTime.Now;

            Assert.IsTrue(_loggerManagement.MessageReceivedEvent.IsSet, "No log message received");

            ILogMessage msg = _loggerManagement.Messages.First();

            Assert.IsInstanceOf<LogMessage>(msg);
            Assert.AreEqual(string.Format(LogMsgFormat, LogMsgArgument), msg.Message, "Log message");
            Assert.AreEqual(_logger, msg.Logger, "Logger");
            Assert.AreEqual(_module.GetType().Name, msg.ClassName, "Message class");
            Assert.AreEqual(LogLevel.Info, msg.Level, "Log Level");
            Assert.GreaterOrEqual(msg.Timestamp, t1, "Start of time interval");
            Assert.LessOrEqual(msg.Timestamp, t2, "End of time interval");

            LogMessage internalMsg = (LogMessage)msg;

            Assert.IsFalse(internalMsg.IsException, "IsException");
            Assert.Null(internalMsg.Exception, "Exception");
            Assert.NotNull(internalMsg.LoggerMessage, "LoggerMessage");
            Assert.IsTrue(internalMsg.LoggerMessage.Contains(_module.GetType().Name), "LoggerMessage Classname");
            Assert.IsTrue(internalMsg.LoggerMessage.Contains(string.Format(LogMsgFormat, LogMsgArgument)), "LoggerMessage Content");
        }

        [Test]
        public void TestLogException()
        {
            DateTime t1 = DateTime.Now;

            _logger.LogException(LogLevel.Error, new Exception(ExceptionMsg), LogMsgFormat, LogMsgArgument);

            _loggerManagement.MessageReceivedEvent.Wait(1000);

            DateTime t2 = DateTime.Now;

            Assert.IsTrue(_loggerManagement.MessageReceivedEvent.IsSet, "No log message received");

            ILogMessage msg = _loggerManagement.Messages.First();

            Assert.IsInstanceOf<LogMessage>(msg);
            Assert.IsTrue(msg.Message.Contains(string.Format(LogMsgFormat, LogMsgArgument)), "Log message");
            Assert.IsTrue(msg.Message.Contains(ExceptionMsg), "Exception message");
            Assert.AreEqual(_module.GetType().Name, msg.ClassName, "Message class");
            Assert.AreEqual(LogLevel.Error, msg.Level, "Log Level");
            Assert.GreaterOrEqual(msg.Timestamp, t1, "Start of time interval");
            Assert.LessOrEqual(msg.Timestamp, t2, "End of time interval");

            LogMessage internalMsg = (LogMessage) msg;

            Assert.IsTrue(internalMsg.IsException, "IsException");
            Assert.NotNull(internalMsg.Exception, "Exception");
            Assert.NotNull(internalMsg.LoggerMessage, "LoggerMessage");
            Assert.IsTrue(internalMsg.LoggerMessage.Contains(_module.GetType().Name), "LoggerMessage Classname");
            Assert.IsTrue(internalMsg.LoggerMessage.Contains(string.Format(LogMsgFormat, LogMsgArgument)), "LoggerMessage Content");
            Assert.IsInstanceOf<Exception>(internalMsg.Exception);
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
        public void BasicEntryLoggingTest(LogLevel senderLevel, LogLevel loggerLevel, bool expectedResult)
        {
            _loggerManagement.SetLevel(_module.Logger, loggerLevel);
            _logger.LogEntry(senderLevel, LogMsgFormat, LogMsgArgument);

            _loggerManagement.MessageReceivedEvent.Wait(EventWaitTime);

            Assert.AreEqual(expectedResult, _loggerManagement.MessageReceivedEvent.IsSet, "Log message received");
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
        public void BasicExceptionLoggingTest(LogLevel senderLevel, LogLevel loggerLevel, bool expectedResult)
        {
            _loggerManagement.SetLevel(_module.Logger, loggerLevel);
            _logger.LogException(senderLevel, new Exception(ExceptionMsg), LogMsgFormat, LogMsgArgument);

            _loggerManagement.MessageReceivedEvent.Wait(EventWaitTime);

            Assert.AreEqual(expectedResult, _loggerManagement.MessageReceivedEvent.IsSet, "Log message received");
        }
    }

}