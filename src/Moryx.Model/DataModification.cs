// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.Model
{
    /// <summary>
    /// DTO for modified data
    /// </summary>
    /// <typeparam name="T">The payload type</typeparam>
    [DataContract]
    public abstract class DataModification<T>
    {
        /// <summary>
        /// The payload
        /// </summary>
        [DataMember]
        public T Data { get; set; }

        /// <summary>
        /// The typ of modification
        /// </summary>
        [DataMember]
        public ModificationType Modification { get; set; }
    }

    /// <summary>
    /// DTO for modified entities
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class ModifiedEntity<TEntity> : DataModification<TEntity>, IEquatable<ModifiedEntity<TEntity>>
        where TEntity : class, IEntity
    {
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ModifiedEntity<TEntity> other)
        {
            if (other == null)
                return false;
            return Data.Id == other.Data.Id;
        }
    }

    /// <summary>
    /// DTO for a modification performed on an entity with the given id
    /// </summary>
    public class ModifiedId : DataModification<long>, IEquatable<ModifiedId>
    {
        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(ModifiedId other)
        {
            if (other == null)
                return false;
            return Data == other.Data;
        }
    }

    /// <summary>
    /// Type of modification
    /// Flags: Delete Update Insert
    /// </summary>
    [Flags, DataContract]
    public enum ModificationType
    {
        /// <summary>The entry was recently inserted</summary>
        [EnumMember]
        Insert = 0x01,

        /// <summary>The entry was modified</summary>
        [EnumMember]
        Update = 0x02,

        /// <summary>The entry was flagged as deleted</summary>
        [EnumMember]
        Delete = 0x04,
    }
}
