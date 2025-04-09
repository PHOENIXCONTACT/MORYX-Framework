// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Runtime.Serialization;

namespace Moryx.AbstractionLayer.Capabilities
{
    /// <summary>
    /// Null object implementation for capabilities to remove null checks.
    /// They provide nothing, but are provided by everything
    /// </summary>
    [DataContract]
    public class NullCapabilities : ICapabilities
    {
        private static NullCapabilities _instance;
        /// <summary>
        /// Singleton instance
        /// </summary>
        public static NullCapabilities Instance => _instance ?? (_instance = new NullCapabilities());

        /// <summary>
        /// Private constructor to enforce singleton
        /// </summary>
        private NullCapabilities()
        {
        }

        /// <inheritdoc />
        public bool IsCombined => false;

        /// <summary>
        /// <see cref="NullCapabilities"/> are provided by everything
        /// </summary>
        public bool ProvidedBy(ICapabilities provided)
        {
            return true;
        }

        /// <summary>
        /// <see cref="NullCapabilities"/> provide nothing, but themselves
        /// </summary>
        public bool Provides(ICapabilities required)
        {
            return false;
        }

        /// <inheritdoc />
        public IEnumerable<ICapabilities> GetAll()
        {
            yield return this;
        }
    }
}
