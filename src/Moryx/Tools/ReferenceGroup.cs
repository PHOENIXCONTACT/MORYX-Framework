// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Reflection;

namespace Moryx.Tools
{
    /// <summary>
    /// Simple helper to implement IGrouping
    /// </summary>
    internal class ReferenceGroup<TReference> : IGrouping<PropertyInfo, TReference>
    {
        /// <summary>
        /// Key of the group
        /// </summary>
        public PropertyInfo Key { get; }

        /// <summary>
        /// Values of the group
        /// </summary>
        private IEnumerable<TReference> Values { get; }

        public ReferenceGroup(PropertyInfo key, TReference value)
        {
            Key = key;
            Values = value == null ? [] : [value];
        }

        public ReferenceGroup(PropertyInfo key, IEnumerable<TReference> values)
        {
            Key = key;
            Values = values ?? [];
        }

        public IEnumerator<TReference> GetEnumerator()
        {
            return (Values ?? []).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}