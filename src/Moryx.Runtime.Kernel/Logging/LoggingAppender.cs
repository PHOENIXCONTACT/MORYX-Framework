// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Logging;
using Moryx.Runtime.Logging;
using Moryx.Threading;

namespace Moryx.Runtime.Kernel.Logging
{
    /// <summary>
    /// Service contract for appending logging features
    /// </summary>
    public class LoggingAppender : ILoggingAppender
    {
        private readonly IServerLoggerManagement _serverLoggerManagement;
        private readonly Dictionary<int, RemoteAppender> _remoteAppenders = new Dictionary<int, RemoteAppender>();
        private readonly int _appenderTimeOut = 30000;
        private readonly IParallelOperations _parallelOperations;

        /// <summary>
        /// Service contract for appending logging features
        /// </summary>
        public LoggingAppender(IServerLoggerManagement serverLoggerManagement, IParallelOperations parallelOperations)
        {
            _serverLoggerManagement = serverLoggerManagement;
            _parallelOperations = parallelOperations;
            _parallelOperations.ScheduleExecution(RemoveDeadAppenders, 100, 10000);
        }

        /// <summary>
        /// Add a remote appender to the logging stream
        /// </summary>
        public int AddRemoteLogAppender(string name, LogLevel level)
        {
            var appender = new RemoteAppender();
            if (string.IsNullOrEmpty(name) && level == LogLevel.Trace)
                _serverLoggerManagement.AppendListenerToStream(appender.BufferMessage);
            else if (string.IsNullOrEmpty(name))
                _serverLoggerManagement.AppendListenerToStream(appender.BufferMessage, level);
            else if (level == LogLevel.Trace)
                _serverLoggerManagement.AppendListenerToStream(appender.BufferMessage, name);
            else
                _serverLoggerManagement.AppendListenerToStream(appender.BufferMessage, level, name);

            var id = 1;
            lock (_remoteAppenders)
            {
                int[] takenIds = _remoteAppenders.Keys.ToArray();
                // Find first non taken id
                while (takenIds.Contains(id))
                    id++;
                _remoteAppenders[id] = appender;
            }

            return id;
        }

        /// <summary>
        /// Check if id belongs to a valid appender
        /// </summary>
        public bool ValidAppender(int appenderId)
        {
            lock (_remoteAppenders)
                return _remoteAppenders.ContainsKey(appenderId);
        }

        /// <summary>
        /// Flush all new messages of this appender
        /// </summary>
        public IEnumerable<ILogMessage> FlushMessages(int appender)
        {
            RemoteAppender targetAppender;
            lock (_remoteAppenders)
                targetAppender = _remoteAppenders.ContainsKey(appender) ? _remoteAppenders[appender] : null;
            return targetAppender == null ? Enumerable.Empty<ILogMessage>() : targetAppender.FlushMessages();
        }

        /// <summary>
        /// Remove a remote appender from the logging stream
        /// </summary>
        /// <param name="appenderId"></param>
        public void RemoveRemoteLogAppender(int appenderId)
        {
            RemoteAppender appender;
            lock (_remoteAppenders)
                appender = _remoteAppenders.ContainsKey(appenderId) ? _remoteAppenders[appenderId] : null;

            if (appender == null)
                return;

            _serverLoggerManagement.RemoveListenerFromStream(appender.BufferMessage);
        }

        private void RemoveDeadAppenders()
        {
            IEnumerable<KeyValuePair<int, RemoteAppender>> appenders;
            lock (_remoteAppenders)
                appenders = _remoteAppenders.Where(appender 
                    => (DateTime.Now - appender.Value.LastFlush).TotalMilliseconds > _appenderTimeOut);

            foreach (var appender in appenders)
                _serverLoggerManagement.RemoveListenerFromStream(appender.Value.BufferMessage);

            lock (_remoteAppenders)
                foreach (var appender in appenders)
                    _remoteAppenders.Remove(appender.Key);
        }
    }
}
