// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Orders.Restrictions;

namespace Moryx.Orders
{
    /// <summary>
    /// Event args for a begin request of an operation
    /// </summary>
    public class OperationBeginRequestEventArgs : OperationActionRequestEventArgs<BeginRestriction>
    {
        /// <summary>
        /// Creates a new instance of <see cref="OperationBeginRequestEventArgs"/>
        /// </summary>
        public OperationBeginRequestEventArgs(Operation operation) : base(operation)
        {
        }
    }
}