// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Container;

namespace Moryx.Serialization
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
        [Obsolete("Replaced by PossibleValues with access to global service registration")]
        public virtual IEnumerable<string> GetValues(IContainer container)
        {
            return Array.Empty<string>();
        }

        /// <summary>
        /// Extract possible values from local or global DI registration
        /// </summary>
        // TODO: Make abstract in MORYX 10
        public virtual IEnumerable<string> GetValues(IContainer localContainer, IServiceProvider serviceProvider)
        {
            return GetValues(localContainer);
        }

        /// <summary>
        /// Parse value from string using local or global DI container
        /// </summary>
        /// <param name="container">Module local DI container</param>
        /// <param name="serviceProvider">Global service registration</param>
        /// <param name="value">Value to parse</param>
        public virtual object Parse(IContainer container, IServiceProvider serviceProvider, string value)
        {
            return value;
        }
    }
}
