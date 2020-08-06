// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;

namespace Moryx.Model
{
    /// <summary>
    /// Interface for all entities
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Database key of this entity
        /// </summary>
        long Id { get; set; }

        /// <summary>
        /// Event if the id was changed
        /// </summary>
        event EventHandler IdChanged;
    }

    /// <summary>
    /// Interface for entities with time tracking of performed modifications
    /// </summary>
    public interface IModificationTrackedEntity : IEntity
    {
        /// <summary>
        /// Time this entity was created
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// Time this entity was last updated
        /// </summary>
        DateTime Updated { get; set; }

        /// <summary>
        /// Time this entity was deleted
        /// </summary>
        DateTime? Deleted { get; set; }
    }
}
