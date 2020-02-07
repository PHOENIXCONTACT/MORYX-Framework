// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Moryx.Logging;

namespace Moryx.Runtime.Maintenance.Logging
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
