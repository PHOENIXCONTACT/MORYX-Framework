using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Interface to provide nice API for single part access
    /// </summary>
    public interface IPartWrapper<T>
        where T : Article
    {
        /// <summary>
        /// Get or set single part
        /// </summary>
        T Part { get; set; }
    }

    /// <summary>
    /// Wrapper structure to read and write single value
    /// from parts collection without heap allocation
    /// </summary>
    internal struct SinglePart<T> : IPartWrapper<T> 
        where T : Article
    {
        private readonly string _name;
        private readonly ICollection<ArticlePart> _allParts;
        
        ///
        internal SinglePart(ICollection<ArticlePart> allParts, string name)
        {
            _allParts = allParts;
            _name = name;
        }

        /// 
        public T Part
        {
            get
            {
                var part = _allParts.SingleOrDefault(NameMatch);
                return part == null ? null : (T)part.Article;
            }
            set
            {
                var match = _allParts.FirstOrDefault(NameMatch);
                if (match == null && value != null) // Initial set
                    _allParts.Add(new ArticlePart(_name, value));
                else if (match != null && value == null) // Remove of exisiting value
                    _allParts.Remove(match);
                else if (value != null) // Override of existing value
                    match.Article = value;
                // Last case (match == null && value == null) is ignored on purpose
            }
        }

        private bool NameMatch(ArticlePart part)
        {
            return part.Name == _name;
        }
    }

    /// <summary>
    /// Wrapper structure to access the parts collection
    /// without heap allocation
    /// </summary>
    internal struct MultipleParts<T> : ICollection<T>
        where T : Article
    {
        private readonly string _name;
        private readonly ICollection<ArticlePart> _allParts;

        /// 
        internal MultipleParts(ICollection<ArticlePart> allParts, string name)
        {
            _allParts = allParts;
            _name = name;
        }

        ///
        public void Add(T item)
        {
            _allParts.Add(new ArticlePart(_name, item));
        }

        ///
        public void Clear()
        {
            foreach (var articlePart in Filtered().ToArray())
            {
                _allParts.Remove(articlePart);
            }
        }

        ///
        public bool Contains(T item)
        {
            return _allParts.Contains(new ArticlePart(_name, item));
        }

        ///
        public void CopyTo(T[] array, int arrayIndex)
        {
            foreach (var part in FilteredParts())
            {
                array[arrayIndex++] = part;
            }
        }

        /// 
        public bool Remove(T item)
        {
            return _allParts.Remove(new ArticlePart(_name, item));
        }

        /// 
        public int Count
        {
            get
            {
                return _allParts.Count(NameMatch);
            }
        }

        /// 
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// 
        public IEnumerator<T> GetEnumerator()
        {
            return FilteredParts().GetEnumerator();
        }

        /// 
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<ArticlePart> Filtered()
        {
            return _allParts.Where(NameMatch);
        }

        private IEnumerable<T> FilteredParts()
        {
            return Filtered().Select(p => p.Article).OfType<T>();
        }

        private bool NameMatch(ArticlePart part)
        {
            return part.Name == _name;
        }

        public static void Replace(ICollection<ArticlePart> allParts, string name, ICollection<T> items)
        {
            var dummy = new MultipleParts<T>(allParts, name);
            dummy.Clear();

            foreach (var item in items)
            {
                dummy.Add(item);
            }
        }
    }
}