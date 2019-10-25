namespace Marvin.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of a <see cref="ProductType"/>
    /// </summary>
    public class DummyProductType : ProductType
    {
        /// <inheritdoc />
        public override string Type => nameof(DummyProductType);

        /// <inheritdoc />
        protected override ProductInstance Instantiate()
        {
            return new DummyProductInstance();
        }
    }
}