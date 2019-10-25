namespace Marvin.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of an article. Created by the <see cref="DummyProductType"/>
    /// </summary>
    public class DummyProductInstance : ProductInstance
    {
        /// <inheritdoc />
        public override string Type => nameof(DummyProductInstance);
    }
}
