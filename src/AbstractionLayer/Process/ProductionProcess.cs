namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Concrete process of producing a single article
    /// </summary>
    public class ProductionProcess : Process
    {
        /// <summary>
        /// The article produced by this process.
        /// </summary>
        public ProductInstance ProductInstance { get; set; }
    }
}
