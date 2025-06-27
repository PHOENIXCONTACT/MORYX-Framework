// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;

namespace Moryx.Orders
{
    /// <summary>
    /// Base class for all order classes
    /// </summary>
    public class Order
    {
        /// <summary>
        /// Number of the order
        /// </summary>
        public virtual string Number { get; protected set; }

        /// <summary>
        /// The type of the order
        /// </summary>
        public virtual string Type { get; protected set; }

        /// <summary>
        /// List of operations for this order
        /// </summary>
        public virtual IReadOnlyList<Operation> Operations { get; protected set; }
    }
}