// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model
{
    /// <summary>
    /// Base class for <see cref="IModificationTrackedEntity"/>.
    /// Created, Updated and Deleted columns will be added.
    /// </summary>
    public abstract class ModificationTrackedEntityBase : EntityBase, IModificationTrackedEntity
    {
        /// <inheritdoc />
        public virtual DateTime Created { get; set; }

        /// <inheritdoc />
        public virtual DateTime Updated { get; set; }

        /// <inheritdoc />
        public virtual DateTime? Deleted { get; set; }
    }
}
