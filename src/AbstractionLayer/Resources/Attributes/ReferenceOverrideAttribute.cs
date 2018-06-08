using System;

namespace Marvin.AbstractionLayer.Resources
{
    /// <summary>
    /// Attribute to decorate properties that override the lower type bound of base class references. Those properties only
    /// and improve type safety of the references
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReferenceOverrideAttribute : Attribute
    {
        /// <summary>
        /// Name of the source property
        /// </summary>
        public string Source { get; private set; }

        /// <summary>
        /// Automatically save changes to the collection
        /// </summary>
        public bool AutoSave { get; set; }

        /// <summary>
        /// Create a new reference override from a source property
        /// </summary>
        /// <param name="source"></param>
        public ReferenceOverrideAttribute(string source)
        {
            Source = source;
        }
    }
}