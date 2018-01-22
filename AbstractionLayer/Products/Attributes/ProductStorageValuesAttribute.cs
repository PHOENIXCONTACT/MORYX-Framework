using System;
using Marvin.Testing;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// This property uses a range of possible values from the database
    /// </summary>
    [OpenCoverIgnore]
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ProductStorageValuesAttribute : Attribute
    {
        /// <summary>
        /// Link this product to storage values with the following key
        /// </summary>
        /// <param name="key"></param>
        public ProductStorageValuesAttribute(string key)
        {
            Key = key;
        }

        /// <summary>
        /// Key of the storage values
        /// </summary>
        public string Key { get; private set; }
    }
}