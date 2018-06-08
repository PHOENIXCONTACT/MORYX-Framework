namespace Marvin.AbstractionLayer
{
    /// <summary>
    /// Concrete process of producing a single article
    /// </summary>
    public class ProductionProcess : Process
    {
        /// <summary>
        /// Unique type name of this instance
        /// </summary>
        public override string Type => nameof(ProductionProcess);

        /// <summary>
        /// The article produced by this process.
        /// </summary>
        public Article Article { get; set; }
    }
}
