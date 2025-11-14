// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using Moryx.Model;

namespace Moryx.Products.Management.Model;

public class ProductTypeEntity : ModificationTrackedEntityBase
{
    public virtual string TypeName { get; set; }

    public virtual string Identifier { get; set; }

    public virtual short Revision { get; set; }

    public virtual string Name { get; set; }

    public virtual long CurrentVersionId { get; set; }

    public virtual ICollection<PartLinkEntity> Parts { get; set; }

    public virtual ICollection<PartLinkEntity> Parents { get; set; }

    public virtual ICollection<ProductRecipeEntity> Recipes { get; set; }

    public virtual ICollection<ProductTypePropertiesEntity> OldVersions { get; set; }

    public virtual ProductTypePropertiesEntity CurrentVersion { get; protected internal set; }

    public ProductTypeEntity()
    {
        Parts = new List<PartLinkEntity>();
        Parents = new List<PartLinkEntity>();
        Recipes = new List<ProductRecipeEntity>();
        OldVersions = new List<ProductTypePropertiesEntity>();
    }

    /// <summary>
    /// Creates a link to the current version of this product's properties.
    /// </summary>
    public void SetCurrentVersion(ProductTypePropertiesEntity properties)
    {
        if (CurrentVersion == properties)
            return;

        if (CurrentVersion != null)
            OldVersions.Add(CurrentVersion);

        CurrentVersion = properties;
    }
}
