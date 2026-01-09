// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Moryx.Orders;

/// <summary>
/// Information about a part of the product which should be produced for an operation
/// </summary>
public class PartCreationContext
{
    /// <summary>
    /// Name of the part like the product name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Identifier of this part
    /// </summary>
    public string Number { get; set; }

    /// <summary>
    /// Quantity of this part which are necessary to produce one product
    /// </summary>
    public double Quantity { get; set; }

    /// <summary>
    /// Unit of the quantity
    /// </summary>
    public string Unit { get; set; }

    /// <summary>
    /// Staging indicator
    /// </summary>
    public StagingIndicator StagingIndicator { get; set; }

    /// <summary>
    /// Classification of this part
    /// </summary>
    public PartClassification Classification { get; set; }
}