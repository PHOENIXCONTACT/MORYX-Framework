using System;

namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Exception thrown when a product for a certain id was not found
    /// </summary>
    public class ProductNotFoundException : Exception
    {
        /// <summary>
        /// Initialize exception with database id
        /// </summary>
        /// <param name="id">Id that was not found</param>
        public ProductNotFoundException(long id)
            : base($"Product not found for id: {id}")
        {
        }
    }
}