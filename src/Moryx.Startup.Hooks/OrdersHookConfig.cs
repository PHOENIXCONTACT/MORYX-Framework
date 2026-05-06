// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Startup.Hooks;

public class OrdersHookConfig
{
    public class ImporterConfig
    {
        /// <summary>
        /// Allows disabling this config entry
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Only run this hook, when the database contains no operations 
        /// </summary>
        public bool OnlyOnFreshDb { get; set; }

        /// <summary>
        /// Name of the operation
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Operation number
        /// </summary>
        public string? Number { get; set; }

        /// <summary>
        /// Number of the order containing this operation
        /// </summary>
        public string? OrderNumber { get; set; }

        /// <summary>
        /// Maps to OrderCreationContext.OrderType
        /// </summary>
        public string OrderType { get; set; } = "default";

        /// <summary>
        /// Identifier of the product to produce
        /// </summary>
        public string? ProductIdentifier { get; set; }
        /// <summary>
        /// Revision of the product to produce
        /// </summary>
        public short ProductRevision { get; set; }
        /// <summary>
        /// Amount to produce
        /// </summary>
        public int TotalAmount { get; set; }
        /// <summary>
        /// Underdelivery target
        /// </summary>
        public int UnderDelivery { get; set; }
        /// <summary>
        /// OverdeliveryTarget
        /// </summary>
        public int OverDelivery { get; set; }
        /// <summary>
        /// Id of the recipe
        /// </summary>
        public long RecipePreselection { get; set; }

        /// <summary>
        /// Unit that the amount is based on
        /// </summary>
        public string? Unit { get; set; } = "pieces";
    }

    /// <summary>
    /// List of operations to create
    /// </summary>
    public ImporterConfig[] Operations { get; set; } = [];
}
