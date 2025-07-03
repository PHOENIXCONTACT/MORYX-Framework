using System.Collections.Concurrent;
using System.Collections.Generic;
using Moryx.Container;
using Moryx.Logging;

namespace Moryx.Orders.Management
{
    [Component(LifeCycle.Singleton, typeof(IOperationLoggerProvider))]
    internal class OperationLoggerProvider : IOperationLoggerProvider
    {
        #region Dependencies

        [UseChild("Operations")]
        public IModuleLogger Logger { get; set; }

        #endregion

        #region Fields and Properties

        private readonly IDictionary<IOperationData, IOperationLogger> _logger = new ConcurrentDictionary<IOperationData, IOperationLogger>();

        #endregion

        public IOperationLogger GetLogger(IOperationData operationData)
        {
            IOperationLogger operationLogger;
            if (_logger.ContainsKey(operationData))
                operationLogger = _logger[operationData];
            else
            {
                operationLogger = new OperationLogger(Logger, operationData);
                _logger[operationData] = operationLogger;
            }

            return operationLogger;
        }

        public void RemoveLogger(IOperationData operationData)
        {
            if (!_logger.ContainsKey(operationData))
                return;

            _logger.Remove(operationData);
        }
    }
}
