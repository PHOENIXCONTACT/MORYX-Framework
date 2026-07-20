// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Moryx.Orders.Management;

/// <summary>
/// Configuration for a single operation to be imported during startup
/// </summary>
[DataContract]
public class OperationImportConfig
{
    /// <summary>
    /// Allows disabling this config entry
    /// </summary>
    [DataMember]
    public bool Disabled { get; set; }

    /// <summary>
    /// Only import this operation when the database contains no existing operations
    /// </summary>
    [DataMember]
    public bool OnlyOnEmptyDb { get; set; }

    /// <summary>
    /// Name of the operation
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Operation number
    /// </summary>
    [DataMember]
    public string Number { get; set; }

    /// <summary>
    /// Number of the order containing this operation
    /// </summary>
    [DataMember]
    public string OrderNumber { get; set; }

    /// <summary>
    /// Type of the order, e.g. 'default'
    /// </summary>
    [DataMember]
    [DefaultValue("default")]
    public string OrderType { get; set; }

    /// <summary>
    /// Identifier of the product to produce
    /// </summary>
    [DataMember]
    public string ProductIdentifier { get; set; }

    /// <summary>
    /// Revision of the product to produce
    /// </summary>
    [DataMember]
    public short ProductRevision { get; set; }

    /// <summary>
    /// Amount to produce
    /// </summary>
    [DataMember]
    public int TotalAmount { get; set; }

    /// <summary>
    /// Allowed under-delivery amount
    /// </summary>
    [DataMember]
    public int UnderDelivery { get; set; }

    /// <summary>
    /// Allowed over-delivery amount
    /// </summary>
    [DataMember]
    public int OverDelivery { get; set; }

    /// <summary>
    /// Id of the preselected recipe
    /// </summary>
    [DataMember]
    public long RecipePreselection { get; set; }

    /// <summary>
    /// Unit that the amount is based on
    /// </summary>
    [DataMember]
    [DefaultValue("pieces")]
    public string Unit { get; set; }
}
