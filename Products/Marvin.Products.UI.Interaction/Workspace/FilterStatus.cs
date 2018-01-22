using System.Text.RegularExpressions;

namespace Marvin.Products.UI.Interaction
{
    /// <summary>
    /// Current filter applied on the product list
    /// </summary>
    internal struct FilterStatus
    {
        /// <summary>
        /// Current search text of the filter
        /// </summary>
        private readonly string _currentSearch;

        /// <summary>
        /// Last search text of the filter
        /// </summary>
        private readonly string _lastSearch;

        private Regex _searchRegex;

        /// <summary>
        /// Create struct with values for last and current search text
        /// </summary>
        private FilterStatus(string currentSearch, string lastSearch)
        {
            _currentSearch = currentSearch;
            _lastSearch = lastSearch;
            _searchRegex = null;
        }

        /// <summary>
        /// Filter products with a new text
        /// </summary>
        public FilterStatus Search(string text)
        {
            return new FilterStatus(text, _currentSearch);
        }

        /// <summary>
        /// Current search text of the filter
        /// </summary>
        public string SearchText => _currentSearch;

        /// <summary>
        /// Check if the products name matches the filter
        /// </summary>
        public bool IsMatch(string name)
        {
            if (_searchRegex == null)
            {
                _searchRegex = new Regex(_currentSearch, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
            }
            return _searchRegex.IsMatch(name);
        }

        /// <summary>
        /// Indicator if the filter has changed
        /// </summary>
        public bool FilterChanged => _currentSearch != _lastSearch;

        /// <summary>
        /// Indicator if current filter extends the old one
        /// </summary>
        public bool FilterExtended => _currentSearch.Contains(_lastSearch ?? string.Empty);

        /// <summary>
        /// Flag if the filter is required at all
        /// </summary>
        public bool FilterRequired => !string.IsNullOrWhiteSpace(_currentSearch);
    }
}