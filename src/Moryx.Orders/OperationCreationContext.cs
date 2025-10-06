// Copyright (c) 2021, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Orders
{
    /// <summary>
    /// Creation context for an operation
    /// </summary>
    [DataContract]
    public class OperationCreationContext
    {
        /// <summary>
        /// Creates a new instance of the <see cref="OperationCreationContext"/>
        /// </summary>
        public OperationCreationContext()
        {
            MaterialParameters = new List<IMaterialParameter>();
            Parts = new List<PartCreationContext>();
        }

        /// <summary>
        /// Order of this operation
        /// </summary>
        [DataMember]
        public OrderCreationContext Order { get; set; }

        /// <summary>
        /// The amount to produce
        /// </summary>
        [DataMember]
        public int TotalAmount { get; set; }

        /// <summary>
        /// Operation name
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The number of this operation
        /// </summary>
        [DataMember]
        public string Number { get; set; }

        /// <summary>
        /// Main product identifier
        /// </summary>
        [DataMember]
        public string ProductIdentifier { get; set; }

        /// <summary>
        /// Revision of the Product
        /// </summary>
        [DataMember]
        public short ProductRevision { get; set; }

        /// <summary>
        /// Name of the Product
        /// </summary>
        [DataMember]
        public string ProductName { get; set; }

        /// <summary>
        /// Id of the preselected recipe
        /// </summary>
        [DataMember]
        public long RecipePreselection { get; set; }

        /// <summary>
        /// Total amount planned for over delivery
        /// </summary>
        [DataMember]
        public int OverDeliveryAmount { get; set; }

        /// <summary>
        /// Total amount planned for under delivery
        /// </summary>
        [DataMember]
        public int UnderDeliveryAmount { get; set; }

        /// <summary>
        /// Expected start time
        /// </summary>
        [DataMember]
        public DateTime PlannedStart { get; set; }

        /// <summary>
        /// Expected end time
        /// </summary>
        [DataMember]
        public DateTime PlannedEnd { get; set; }

        /// <summary>
        /// Target cycle time for the production of the depending product
        /// </summary>
        [DataMember]
        public double TargetCycleTime { get; set; }

        /// <summary>
        /// Unit of product
        /// </summary>
        [DataMember]
        public string Unit { get; set; }

        /// <summary>
        /// Target stock for produced parts
        /// </summary>
        [DataMember]
        public string TargetStock { get; set; }

        /// <summary>
        /// Part list of the product of this operation
        /// </summary>
        public ICollection<PartCreationContext> Parts { get; set; }

        /// <summary>
        /// Additional operation depending parameters for the production
        /// </summary>
        public ICollection<IMaterialParameter> MaterialParameters { get; set; }
    }
}