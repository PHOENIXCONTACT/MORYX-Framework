namespace Marvin.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of a <see cref="Product"/>
    /// </summary>
    public class DummyProduct : Product
    {
        /// <inheritdoc />
        public override string Type => nameof(DummyProduct);

        /// <inheritdoc />
        protected override Article Instantiate()
        {
            return new DummyArticle();
        }
    }
}