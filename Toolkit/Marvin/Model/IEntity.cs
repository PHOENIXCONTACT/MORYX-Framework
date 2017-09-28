using System;
using System.ComponentModel;

namespace Marvin.Model
{
    /// <summary>
    /// Interface for all entities
    /// </summary>
    public interface IEntity : INotifyPropertyChanged
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
        DateTime Created { get; }

        /// <summary>
        /// Thime this entity was last updatedt
        /// </summary>
        DateTime Updated { get; }

        /// <summary>
        /// Time this entity was deleted
        /// </summary>
        DateTime? Deleted { get; }
    }
}