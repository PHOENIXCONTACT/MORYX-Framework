// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.ComponentModel.DataAnnotations;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Properties;

namespace Moryx.AbstractionLayer.Identity;

/// <summary>
/// Identity to assign on <see cref="ProductInstance"/> to identify batches
/// </summary>
[Display(Name = nameof(Strings.BatchIdentity_Name), Description = nameof(Strings.BatchIdentity_Description), ResourceType = typeof(Strings))]
public class BatchIdentity : IIdentity
{
    /// <inheritdoc />
    [Display(Name = nameof(Strings.BatchIdentity_Identifier_Name), Description = nameof(Strings.BatchIdentity_Identifier_Description), ResourceType = typeof(Strings))]
    public string Identifier { get; private set; }

    /// <summary>
    /// Creates a new batch identity with the given batch identifier
    /// </summary>
    public BatchIdentity(string identifier)
    {
        Identifier = identifier;
    }

    /// <inheritdoc />
    public void SetIdentifier(string identifier)
    {
        Identifier = identifier;
    }

    /// <inheritdoc />
    public bool Equals(IIdentity other)
    {
        return other is BatchIdentity batchIdentity && batchIdentity.Identifier == Identifier;
    }
}