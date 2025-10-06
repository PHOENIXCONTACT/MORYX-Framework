// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders.Management
{
    /// <summary>
    /// Event args for a advice
    /// </summary>
    internal class AdviceEventArgs : OperationEventArgs
    {
        /// <summary>
        /// Information about the last advice
        /// </summary>
        public OperationAdvice Advice { get; }

        /// <summary>
        /// Creates a new instance of <see cref="AdviceEventArgs"/>
        /// </summary>
        public AdviceEventArgs(IOperationData operationData, OperationAdvice advice) : base(operationData)
        {
            Advice = advice;
        }
    }
}

