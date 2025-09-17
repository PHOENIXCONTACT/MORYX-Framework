﻿using Moryx.Tools.FunctionResult;
using System;

namespace Moryx.Communication.Connection
{
    /// <summary>
    /// General interface to model any kind of communication
    /// to and with devices
    /// </summary>
    public interface IDeviceCommunication
    {
        /// <summary>
        /// Starts communication
        /// </summary>
        /// <param name="communicate">The procedure that executes
        /// this communication</param>
        void Start(Func<FunctionResult> communicate);
        
        /// <summary>
        /// Stops the communication
        /// </summary>
        void Stop();

        /// <summary>
        /// Event handler to be invoked when the communication
        /// could be executed.
        /// </summary>
        event EventHandler Executed;

        /// <summary>
        /// Event handler to be invoked when there was any error
        /// during communication
        /// </summary>
        event EventHandler<FunctionResult> Failed;
    }
}