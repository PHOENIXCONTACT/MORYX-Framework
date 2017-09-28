using System;
using System.Collections;

namespace Marvin.Bindings
{
    /// <summary>
    /// Resolver that can extract values from indexes
    /// </summary>
    public class IndexResolver : BindingResolverBase
    {
        private readonly string _index;
        private int _numericIndex;

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
        private Func<object, object> _cachedResolver;
        /// <inheritdoc />
        public sealed override object Resolve(object source)
        {
            if (_cachedResolver == null)
            {
                _cachedResolver = CreateResolver(source);
            }

            var result = _cachedResolver(source);
            return Proceed(result);
        }

        private Func<object, object> CreateResolver(object source)
        {
            Func<object, object> resolver = null;

            if (int.TryParse(_index, out _numericIndex))
            {
                var dict = source as IDictionary;
                var list = source as IList;
                if (dict != null)
                {
                    resolver = FromNumericDictionary;
                }
                else if (list != null)
                {
                    resolver = FromList;
                }
            }

            // Numeric failed, try text
            if (resolver == null)
            {
                if (source is IDictionary)
                {
                    resolver = FromTextDictionary;
                }
                else
                {
                    resolver = obj => null;
                }
            }

            return resolver;
        }

        private object FromNumericDictionary(object source)
        {
            var dict = (IDictionary) source;
            return dict.Contains(_numericIndex) ? dict[_numericIndex] : null;
        }

        private object FromTextDictionary(object source)
        {
            var dict = (IDictionary)source;
            return dict.Contains(_index) ? dict[_index] : null;
        }

        private object FromList(object source)
        {
            var list = (IList)source;
            return list.Count > _numericIndex ? list[_numericIndex] : null;
        }
    }
}