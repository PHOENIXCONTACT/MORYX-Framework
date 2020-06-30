// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public ReferenceGroup(PropertyInfo key, params TReference[] values)
        {
            Key = key;
            Values = values;
        }

        public ReferenceGroup(PropertyInfo key, IEnumerable<TReference> values)
        {
            Key = key;
            Values = values;
        }

        public IEnumerator<TReference> GetEnumerator()
        {
            return (Values ?? Enumerable.Empty<TReference>()).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
