// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders
{
    /// <summary>
    /// Event args of an operation advice
    /// </summary>
    public class OperationAdviceEventArgs : OperationChangedEventArgs
    {
        /// <summary>
        /// Original advice
        /// </summary>
        public OperationAdvice Advice { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OperationAdviceEventArgs"/>
        /// </summary>
        public OperationAdviceEventArgs(Operation operation, OperationAdvice advice) : base(operation)
        {
            Advice = advice;
        }
    }
}