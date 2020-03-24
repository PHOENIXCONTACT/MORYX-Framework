// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections.Generic;
using Marvin.Container;

namespace Marvin.Serialization
{
    /// <summary>
    /// Base attribute for all attributes that support multiple values
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
    public abstract class PossibleValuesAttribute : Attribute
    {
        /// <summary>
        /// Flag if this member implements its own string to value conversion
        /// </summary>
        public abstract bool OverridesConversion { get; }

        /// <summary>
        /// Flag if new values shall be updated from the old value
        /// </summary>
        public abstract bool UpdateFromPredecessor { get; }

        /// <summary>
        /// All possible values for this member represented as strings. The given container might be null
        /// and can be used to resolve possible values
        /// </summary>
        public abstract IEnumerable<string> GetValues(IContainer container);

        /// <summary>
        /// String to value conversion
        /// </summary>
        public virtual object Parse(IContainer container, string value)
        {
            return value;
        }
    }
}
