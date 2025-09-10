﻿using Moryx.Container;
using Moryx.Tools.FunctionResult;
using System;

namespace Moryx.Communication.Connection
{
    /// <summary>
    /// Specialized interface of <see cref="IDeviceCommunication"/>
    /// </summary>
    public interface IStatusCheck : IDeviceCommunication;

    /// <summary>
    /// Implements a common process of executing status checks.
    /// </summary>
    [Plugin(LifeCycle.Transient, [typeof(IStatusCheck)])]
    public class StatusCheck : DeviceCommunicationBase, IStatusCheck
    {
        /// <summary>
        /// Interval in which the status check is executed
        /// </summary>
        public int IntervalInMs { get; set; } = 5000;

        /// <summary>
        /// Starts intervally checking the status
        /// </summary>
        public override void Start(Func<FunctionResult> execute)
        {
            if (_started)
                return;

            _execute = execute;
            _started = true;

            StartStatusCheck();
        }

        private void StartStatusCheck()
        {
            _timerId = ParallelOperations.ScheduleExecution(
                CheckStatus,
                IntervalInMs,
                0);
        }

        private void CheckStatus()
        {
            if (_execute != null)
            {
                var result = _execute.Invoke();
                if (result.Success)
                {
                    RaiseExecutedEvent();
                    StartStatusCheck();
                }
                else
                {
                    RaiseFailureEvent(result);
                }
            }
        }
    }
}
