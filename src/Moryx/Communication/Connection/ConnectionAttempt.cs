using Moryx.Container;
using Moryx.Tools.FunctionResult;
using System;

namespace Moryx.Communication.Connection
{
    /// <summary>
    /// Specialized interface of <see cref="IDeviceCommunication"/>
    /// </summary>
    public interface IConnectionAttempt : IDeviceCommunication;

    /// <summary>
    /// Implements a common process of establishing connections of any kind
    /// which is defined by the user.
    /// </summary>
    [Plugin(LifeCycle.Transient, [typeof(IConnectionAttempt)])]
    public class ConnectionAttempt : DeviceCommunicationBase, IConnectionAttempt
    {
        /// <summary>
        /// Retry in case a connection couldn't be established
        /// </summary>
        public bool RetryOnFailure { get; set; } = true;

        /// <summary>
        /// Reconnect timeout in milliseconds
        /// </summary>
        public int TimeoutInMs { get; set; } = 10000;

        /// <summary>
        /// Starts trying to connect a connection and checks its status.
        /// </summary>
        public override void Start(Func<FunctionResult> communicate)
        {
            if (_started)
                return;

            _execute = communicate
                       ?? throw new ArgumentException(nameof(communicate));
            _started = true;

            Start();
        }

        private void Start()
        {
            var result = _execute!.Invoke();
            if (result.Success)
            {
                RaiseExecutedEvent();
            }
            else
            {
                RaiseFailureEvent(result);
                if (RetryOnFailure)
                    _timerId = ParallelOperations.ScheduleExecution(
                        Start,
                         TimeoutInMs,
                         0);
            }
        }
    }
}
