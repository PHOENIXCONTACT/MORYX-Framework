using System.Collections.Generic;
using System.Reflection;

namespace Marvin.Configuration
{
    /// <summary>
    /// Settings class for the <see cref="ValueProviderExecutor"/>
    /// </summary>
    public class ValueProviderExecutorSettings
    {
        private readonly List<IValueProvider> _providers = new List<IValueProvider>();

        private readonly List<IValueProviderFilter> _filters = new List<IValueProviderFilter> { new DefaultCanWriteValueProviderFilter() };

        /// <summary>
        /// Configured filters
        /// </summary>
        public IEnumerable<IValueProviderFilter> Filters => _filters;

        /// <summary>
        /// Configured providers
        /// </summary>
        public IEnumerable<IValueProvider> Providers => _providers;

        /// <summary>
        /// Configured <see cref="BindingFlags"/>
        /// </summary>
        public BindingFlags PropertyBindingFlags { get; set; } = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Adds a provider
        /// </summary>
        /// <param name="valueProvider"></param>
        /// <returns></returns>
        public ValueProviderExecutorSettings AddProvider(IValueProvider valueProvider)
        {
            _providers.Add(valueProvider);
            return this;
        }

        /// <summary>
        /// Adds a set of providers
        /// </summary>
        /// <param name="valueProviders"></param>
        /// <returns></returns>
        public ValueProviderExecutorSettings AddProviders(IEnumerable<IValueProvider> valueProviders)
        {
            _providers.AddRange(valueProviders);
            return this;
        }

        /// <summary>
        /// Adds a filter
        /// </summary>
        /// <param name="valueProviderFilter"></param>
        /// <returns></returns>
        public ValueProviderExecutorSettings AddFilter(IValueProviderFilter valueProviderFilter)
        {
            _filters.Add(valueProviderFilter);
            return this;
        }

        /// <summary>
        /// Adds <see cref="DefaultValueProvider"/>
        /// </summary>
        /// <returns></returns>
        public ValueProviderExecutorSettings AddDefaultValueProvider()
        {
            return AddProvider(new DefaultValueProvider());
        }
    }
}