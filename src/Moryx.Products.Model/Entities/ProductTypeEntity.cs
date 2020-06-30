// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Model;
using System.Collections.Generic;

namespace Moryx.Products.Model
{
    public class ProductTypeEntity : ModificationTrackedEntityBase
    {
        public virtual string TypeName { get; set; }

        public virtual string Identifier { get; set; }

        public virtual short Revision { get; set; }

        public virtual string Name { get; set; }

        public virtual long CurrentVersionId { get; set; }

        public virtual ICollection<PartLink> Parts { get; set; }

        public virtual ICollection<PartLink> Parents { get; set; }

        public virtual ICollection<ProductFileEntity> Files { get; set; }

        public virtual ICollection<ProductRecipeEntity> Recipes { get; set; }

        public virtual ICollection<ProductProperties> OldVersions { get; set; }

        public virtual ProductProperties CurrentVersion { get; protected internal set; }

        /// <summary>
        /// Creates a link to the current version of this product's properties.
        /// </summary>
        public void SetCurrentVersion(ProductProperties properties)
        {
            if (CurrentVersion == properties)
                return;

            if (CurrentVersion != null)
                OldVersions.Add(CurrentVersion);

            CurrentVersion = properties;
        }
    }
}
