// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Moryx.Orders
{
    /// <summary>
    /// Creation context for an order
    /// </summary>
    [DataContract(IsReference = true)]
    public class OrderCreationContext
    {
        /// <summary>
        /// Creates a new instance of <see cref="OrderCreationContext"/>
        /// </summary>
        public OrderCreationContext()
        {
            Operations = new List<OperationCreationContext>();
            MaterialParameters = new List<IMaterialParameter>();
        }

        /// <summary>
        /// Order number
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Type of the order e.G. PX23/PX24
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// Material parameters passed directly to the new order
        /// </summary>
        [DataMember]
        public ICollection<IMaterialParameter> MaterialParameters { get; set; }

        /// <summary>
        /// List of all operations for this order
        /// </summary>
        [DataMember]
        public ICollection<OperationCreationContext> Operations { get; set; }
    }
}
