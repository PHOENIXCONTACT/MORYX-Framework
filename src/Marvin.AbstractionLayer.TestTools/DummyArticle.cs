namespace Marvin.AbstractionLayer.TestTools
{
    /// <summary>
    /// Dummy implementation of an article. Created by the <see cref="DummyProduct"/>
    /// </summary>
    public class DummyArticle : Article
    {
        /// <inheritdoc />
        public override string Type => nameof(DummyArticle);
    }
}
