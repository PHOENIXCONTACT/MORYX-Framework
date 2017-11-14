using System.Collections;
using System.Linq;
using System.Reflection;

namespace Marvin
{
    /// <summary>
    /// Internal delegate used to process nodes of the config
    /// </summary>
    /// <param name="parent">Parent object of the currently processed property</param>
    /// <param name="property">Current property info.</param>
    /// <returns>True if this node does not require further processing</returns>
    public delegate bool NodeProcessor(object parent, PropertyInfo property);

    /// <summary>
    /// Provides all available properties on the given object.
    /// </summary>
    public static class ValueProvider
    {
        /// <summary>
        /// Fill all available properties on this object
        /// </summary>
        /// <param name="config">The config which should be processed.</param>
        /// <param name="processors">An amount of NodeProcessors.</param>
        public static void FillProperties(object config, params NodeProcessor[] processors)
        {
            ScanObjectTree(config, processors);
        }

        private static void ScanObjectTree(object target, NodeProcessor[] nodeProcessors)
        {
            foreach (var property in target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Process this node
                // The first processor returning true will terminate the iteration
                if (nodeProcessors.Any(nodeProcessor => nodeProcessor(target, property)))
                    continue;

                var value = property.GetValue(target);

                // Dive deeper
                if (!(value is IEnumerable))
                {
                    ScanObjectTree(value, nodeProcessors);
                    continue;
                }
                // Last option are collections
                foreach (var entry in (IEnumerable)value)
                {
                    ScanObjectTree(entry, nodeProcessors);
                }
            }
        }
    }
}
