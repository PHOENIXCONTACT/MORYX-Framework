// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

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
