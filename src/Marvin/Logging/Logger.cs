using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Marvin.Tools;

namespace Marvin.Logging
{
    internal class Logger : IModuleLogger
    {
        // Logging fields
        private readonly ModuleLoggerConfig _config;

        private readonly ILogTargetFactory _logTargetFactory;
        private readonly ILogTarget _targetLog;

        // Hosting fields
        private readonly string _hostName; // permanent cache for faster access

        /// <summary>
        /// Parent container of this child
        /// </summary>
        public IModuleLogger Parent { get; set; }

        /// <summary>
        /// Constructor invoked by the logger management
        /// </summary>
        /// <param name="config">Top level logger config</param>
        /// <param name="targetType">Type this logger is assigned to</param>
        /// <param name="factory">Factory to create additional logger</param>
        /// <param name="logQueue">Global stream listener</param>
        internal Logger(ModuleLoggerConfig config, Type targetType, ILogTargetFactory factory, BlockingCollection<LogMessage> logQueue)
            : this(config, targetType, factory, null, logQueue)
        {
        }

        /// <summary>
        /// Constructor used for create child
        /// </summary>
        /// <param name="config">Top level logger config</param>
        /// <param name="logQueue">Global stream listener</param>
        /// <param name="factory">Factory to create additional logger</param>
        /// <param name="targetType">Type this logger is assigned to</param>
        /// <param name="logTarget">Existing log instance for loggers of same name</param>
        private Logger(ModuleLoggerConfig config, Type targetType, ILogTargetFactory factory, ILogTarget logTarget, BlockingCollection<LogMessage> logQueue)
        {
            _config = config;
            _logTargetFactory = factory;
            _hostName = targetType.Name;
            _targetLog = logTarget ?? factory.Create(_config.LoggerName);
            LogQueue = logQueue;

            // Put this instance to the clone cache for later reuse.
            _cloneCache[_hostName] = this;
        }

        /// <summary>
        /// Name of this logger instance
        /// </summary>
        public string Name => _config.LoggerName;

        /// <summary>
        /// Active logging level
        /// </summary>
        public LogLevel ActiveLevel
        {
            get { return _config.ActiveLevel; }
            set { _config.ActiveLevel = value; }
        }

        /// <summary>
        /// Add a new entry to the log
        /// </summary>
        public void Log(LogLevel level, string message, params object[] formatParameters)
        {
            if (level < ActiveLevel)
                return;

            var logMessage = new LogMessage(this, _hostName, level, message, formatParameters)
            {
                LogTarget = _targetLog
            };
            LogQueue.Add(logMessage);
        }

        /// <summary>
        /// Log a caught exception
        /// </summary>
        public void LogException(LogLevel level, Exception ex, string message, params object[] formatParameters)
        {
            if (level < ActiveLevel)
                return;

            var logMessage = new LogMessage(this, _hostName, level, ex, message, formatParameters)
            {
                LogTarget = _targetLog
            };
            LogQueue.Add(logMessage);
        }

        /// <summary>
        /// Internal log queue executed by logger management
        /// </summary>
        public BlockingCollection<LogMessage> LogQueue { get; }

        /// <summary>
        /// Create a new 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IModuleLogger GetChild(string name)
        {
            return GetChild(name, CallerId.GetCaller().CallingClass);
        }

        /// <summary>
        /// Get the child with this name
        /// </summary>
        public IModuleLogger GetChild(string name, Type targetType)
        {
            lock (_children)
            {
                // Clone with same name
                if (string.IsNullOrEmpty(name))
                    return Clone(targetType);

                // Clone of existing child
                if (_children.ContainsKey(name))
                    return _children[name].Clone(targetType);

                var childName = $"{Name}.{name}";
                var config = _config.ChildConfigs.FirstOrDefault(item => item.LoggerName == childName);
                if (config == null)
                {
                    config = new ModuleLoggerConfig { LoggerName = childName };
                    _config.ChildConfigs.Add(config);
                }

                _children[name] = new Logger(config, targetType, _logTargetFactory, LogQueue) { Parent = this };

                return _children[name];
            }
        }

        private readonly Dictionary<string, Logger> _cloneCache = new Dictionary<string, Logger>();

        private Logger Clone(Type targetType)
        {
            var cloneName = targetType.Name;
            if (_cloneCache.ContainsKey(cloneName))
                return _cloneCache[cloneName];

            var logger = new Logger(_config, targetType, _logTargetFactory, _targetLog, LogQueue) { Parent = Parent };
            _cloneCache[cloneName] = logger;
            return logger;
        }

        IModuleLogger IModuleLogger.Clone(Type targetType)
        {
            return Clone(targetType);
        }

        internal void ClearChildren()
        {
            lock (_children)
                _children.Clear();
            lock (_cloneCache)
                _cloneCache.Clear();
        }

        #region IEnumerable

        private readonly IDictionary<string, Logger> _children = new Dictionary<string, Logger>();

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IModuleLogger> GetEnumerator()
        {
            lock (_children)
            {
                var copy = _children.Values.ToList();
                return copy.GetEnumerator();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return Name + ": " + ActiveLevel;
        }
    }
}