// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Identity
{
    /// <summary>
    /// Identity for unique numbers like serials or MAC-Addresses
    /// </summary>
    [DataContract]
    public abstract class NumberIdentity : IIdentity
    {
        /// <summary>
        /// Create number identity for a certain type
        /// </summary>
        /// <param name="numberType"></param>
        protected NumberIdentity(int numberType) : this(numberType, string.Empty)
        {
        }

        /// <summary>
        /// Create number identity of type and know identifier
        /// </summary>
        /// <param name="numberType"></param>
        /// <param name="identifier"></param>
        protected NumberIdentity(int numberType, string identifier)
        {
            NumberType = numberType;
            Identifier = identifier;
        }

        /// <inheritdoc />
        [DataMember]
        public string Identifier { get; protected set; }

        /// <summary>
        /// Field for enum values to distinguish between different values
        /// </summary>
        [DataMember]
        public int NumberType { get; protected set; }

        ///
        public virtual void SetIdentifier(string identifier)
        {
            Identifier = identifier;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return Identifier;
        }

        /// <inheritdoc />
        public bool Equals(IIdentity other)
        {
            var casted = other as NumberIdentity;
            return casted != null
                && casted.NumberType.Equals(NumberType)
                && casted.Identifier == Identifier;
        }

        /// <summary>
        /// Compares this object with another one.
        /// </summary>
        /// <param name="obj">The object to compare with</param>
        /// <returns>True, if the values of the objects are the same.</returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as IIdentity);
        }

        /// <summary>
        /// Compares two NumberIdentity objects.
        /// </summary>
        /// <param name="id1">The first NumberIdentity</param>
        /// <param name="id2">The second NumberIdentity</param>
        /// <returns>True, if the values of the objects are the same.</returns>
        public static bool operator ==(NumberIdentity id1, NumberIdentity id2)
        {
            if (((object)id1) == null || ((object)id2) == null)
                return Object.Equals(id1, id2);

            return id1.Equals(id2);
        }

        /// <summary>
        /// Compares two NumberIdentity objects.
        /// </summary>
        /// <param name="id1">The first NumberIdentity</param>
        /// <param name="id2">The second NumberIdentity</param>
        /// <returns>True, if the values of the objects differ.</returns>
        public static bool operator !=(NumberIdentity id1, NumberIdentity id2)
        {
            if (((object)id1) == null || ((object)id2) == null)
                return !Object.Equals(id1, id2);

            return !(id1.Equals(id2));
        }

        /// <summary>
        /// Gets the Hash code of identifier or other number type
        /// </summary>
        public override int GetHashCode()
        {
            return NumberType * (Identifier?.GetHashCode() ?? 42);
        }

    }
}
