using System;
using System.Collections.Generic;
using Marvin.Logging;

namespace Marvin.Runtime.Maintenance.Plugins.LogMaintenance
{
    internal class RemoteAppender
    {
        private readonly object _bufferLock = new object();
        private List<ILogMessage> _bufferedMessages = new List<ILogMessage>();
        public DateTime LastFlush { get; set; }
 
        public void BufferMessage(ILogMessage message)
        {
            lock(_bufferLock)
                _bufferedMessages.Add(message);
        }

        public IEnumerable<ILogMessage> FlushMessages()
        {
            lock (_bufferLock)
            {
                var copy = _bufferedMessages;
                _bufferedMessages = new List<ILogMessage>();
                LastFlush = DateTime.Now;
                return copy;
            }
        }
    }
}
