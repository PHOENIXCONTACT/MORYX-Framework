// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;
using Moryx.Material.Lineage;

namespace Moryx.Material.Integrations.Products;

// TODO: Lineage related aspects are placeholders. The event schema, persistence and
//       recording flow still need to be finalized together with the material management
//       lineage store. See docs/module-material-management/architecture.md.
/// <summary>
/// Lineage event recorded when a product type link is removed from a container.
/// </summary>
[DataContract]
public class ProductTypeUnlinkLineageEvent : LinkLineageEventBase
{
    /// <summary>
    /// Identity of the product type that was previously linked.
    /// </summary>
    [DataMember]
    public string ProductIdentity { get; set; } = string.Empty;
}