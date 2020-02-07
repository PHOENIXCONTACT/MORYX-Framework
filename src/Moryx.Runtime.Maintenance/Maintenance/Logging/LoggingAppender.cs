// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using Moryx.Container;
using Moryx.Logging;
using Moryx.Threading;

namespace Moryx.Runtime.Maintenance.Logging
{
    [Plugin(LifeCycle.Singleton, typeof(ILoggingAppender), Name = PluginName)]
    internal class LoggingAppender : ILoggingAppender, ILoggingComponent
    {
        internal const string PluginName = "LoggingPlugin";

        public IModuleLogger Logger { get; set; }

        public IServerLoggerManagement LoggerManagement { get; set; }

        public IParallelOperations ParallelOperations { get; set; }

        public ModuleConfig ModuleConfig { get; set; }

        public void Start()
        {
            ParallelOperations.ScheduleExecution(CheckDeadAppender, 100, 10000);
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
            lock (_remoteAppenders)
            {
                foreach (var remoteAppender in _remoteAppenders.Values)
                {
                    LoggerManagement.RemoveListenerFromStream(remoteAppender.BufferMessage);
                }
                _remoteAppenders.Clear();
            }
        }

        private void CheckDeadAppender()
        {
            IEnumerable<KeyValuePair<int, RemoteAppender>> appenders;
            lock (_remoteAppenders)
            {
                appenders = _remoteAppenders.Where(appender => (DateTime.Now - appender.Value.LastFlush).TotalMilliseconds > ModuleConfig.AppenderTimeOut)
                                            .ToArray();
            }
            foreach (var appender in appenders)
            {
                LoggerManagement.RemoveListenerFromStream(appender.Value.BufferMessage);
            }
            lock (_remoteAppenders)
            {
                foreach (var appender in appenders)
                {
                    _remoteAppenders.Remove(appender.Key);
                }
            }
        }

        private readonly Dictionary<int, RemoteAppender> _remoteAppenders = new Dictionary<int, RemoteAppender>();

        /// <summary>
        /// Add a remote appender to the logging stream
        /// </summary>
        public int AddRemoteLogAppender(string name, LogLevel level)
        {
            Logger.Log(LogLevel.Info, "Added appender with name {0} and level {1}", name, level);

            var appender = new RemoteAppender();
            if (string.IsNullOrEmpty(name) && level == LogLevel.Trace)
                LoggerManagement.AppendListenerToStream(appender.BufferMessage);
            else if (string.IsNullOrEmpty(name))
                LoggerManagement.AppendListenerToStream(appender.BufferMessage, level);
            else if (level == LogLevel.Trace)
                LoggerManagement.AppendListenerToStream(appender.BufferMessage, name);
            else
                LoggerManagement.AppendListenerToStream(appender.BufferMessage, level, name);

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
            {
                return _remoteAppenders.ContainsKey(appenderId);
            }
        }

        /// <summary>
        /// Flush all new messages of this appender
        /// </summary>
        public IEnumerable<ILogMessage> FlushMessages(int appender)
        {
            RemoteAppender targetAppender;
            lock (_remoteAppenders)
            {
                targetAppender = _remoteAppenders.ContainsKey(appender) ? _remoteAppenders[appender] : null;
            }
            return targetAppender == null ? new ILogMessage[0] : targetAppender.FlushMessages();
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

            LoggerManagement.RemoveListenerFromStream(appender.BufferMessage);
        }
    }
}
