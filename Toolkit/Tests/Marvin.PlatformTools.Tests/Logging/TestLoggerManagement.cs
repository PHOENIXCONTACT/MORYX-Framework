using System.Collections.Generic;
using System.Threading;
using Marvin.Logging;

namespace Marvin.PlatformTools.Tests.Logging
{
    public class TestLoggerManagement : LoggerManagement
    {
        private readonly List<ILogMessage> _messages = new List<ILogMessage>();

        private readonly ManualResetEventSlim _messageReceivedEvent = new ManualResetEventSlim(false);

        public ManualResetEventSlim MessageReceivedEvent { get { return _messageReceivedEvent; } }

        public IEnumerable<ILogMessage> Messages
        {
            get
            {
                lock (_messages)
                {
                    return _messages.ToArray();
                }
            }
        }
 
        protected override ModuleLoggerConfig GetLoggerConfig(string name)
        {
            return new ModuleLoggerConfig
            {
                ActiveLevel = LogLevel.Info,
                LoggerName = name
            };
        }

        protected override void ForwardToListeners(ILogMessage logMessage)
        {
            lock (_messages)
            {
                _messages.Add(logMessage);
                _messageReceivedEvent.Set();
            }
        }

        public void ClearMessages()
        {
            lock (_messages)
            {
                _messages.Clear();
            }
        }
    }
}