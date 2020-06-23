// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System;
using System.Collections;

namespace Moryx.Bindings
{
    /// <summary>
    /// Resolver that can extract values from indexes
    /// </summary>
    public class IndexResolver : BindingResolverBase
    {
        private readonly string _index;

        /// <summary>
        /// Create a new <see cref="IndexResolver"/> with a reflection resolver
        /// to fetch the index source fourst.
        /// </summary>
        public IndexResolver(string index)
        {
            _index = index;
        }

        /// <summary>
        /// Once the source was evaluated once this delegate speeds up resolution
        /// </summary>
        private IIndexStrategy _cachedResolver;
        /// <inheritdoc />
        protected sealed override object Resolve(object source)
        {
            if (_cachedResolver == null)
            {
                _cachedResolver = CreateResolver(source);
            }

            return _cachedResolver.Resolve(source);
        }

        /// <inheritdoc />
        protected sealed override bool Update(object source, object value)
        {
            if (_cachedResolver == null)
            {
                _cachedResolver = CreateResolver(source);
            }

            return _cachedResolver.Update(source, value);
        }

        private IIndexStrategy CreateResolver(object source)
        {
            IIndexStrategy resolver = null;

            var numericIndex = 0;
            if (int.TryParse(_index, out numericIndex))
            {
                var dict = source as IDictionary;
                var list = source as IList;
                if (dict != null)
                {
                    resolver = new NumericDictStrategy(numericIndex);
                }
                else if (list != null)
                {
                    resolver = new ListStrategy(numericIndex);
                }
            }

            // Numeric failed, try text
            if (resolver == null)
            {
                if (source is IDictionary)
                {
                    resolver = new TextDictStrategy(_index);
                }
                else
                {
                    resolver = new NullStrategy();
                }
            }

            return resolver;
        }

        private interface IIndexStrategy
        {
            object Resolve(object source);

            bool Update(object source, object value);
        }

        private class NullStrategy : IIndexStrategy
        {
            public object Resolve(object source)
            {
                return null;
            }

            public bool Update(object source, object value)
            {
                return false;
            }
        }

        private class NumericDictStrategy : IIndexStrategy
        {
            private readonly int _index;

            public NumericDictStrategy(int index)
            {
                _index = index;
            }

            public object Resolve(object source)
            {
                var dict = (IDictionary)source;
                return dict.Contains(_index) ? dict[_index] : null;
            }

            public bool Update(object source, object value)
            {
                var dict = (IDictionary)source;
                dict[_index] = value;
                return true;
            }
        }

        private class TextDictStrategy : IIndexStrategy
        {
            private readonly string _index;

            public TextDictStrategy(string index)
            {
                _index = index;
            }

            public object Resolve(object source)
            {
                var dict = (IDictionary)source;
                return dict.Contains(_index) ? dict[_index] : null;
            }

            public bool Update(object source, object value)
            {
                var dict = (IDictionary)source;
                dict[_index] = value;
                return true;
            }
        }

        private class ListStrategy : IIndexStrategy
        {
            private readonly int _index;

            public ListStrategy(int index)
            {
                _index = index;
            }

            public object Resolve(object source)
            {
                var list = (IList)source;
                return list.Count > _index ? list[_index] : null;
            }

            public bool Update(object source, object value)
            {
                var list = (IList)source;
                list[_index] = value;
                return true;
            }
        }
    }
}
