// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Orders
{
    /// <summary>
    /// Operation depending event args
    /// </summary>
    public class OperationChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Changes operation instance
        /// </summary>
        public Operation Operation { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OperationChangedEventArgs"/>
        /// </summary>
        public OperationChangedEventArgs(Operation operation)
        {
            Operation = operation;
        }
    }
}