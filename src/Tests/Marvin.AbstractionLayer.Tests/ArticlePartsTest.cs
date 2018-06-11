using System.Collections.Generic;
using System.Linq;
using Marvin.Products.Samples;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.Tests
{
    [TestFixture]
    public class ArticlePartsTest
    {
        [Test]
        public void ToParts()
        {
            // Arrange
            var dummy = new WatchArticle
            {
                WatchFace = new WatchFaceArticle(),
                Neddles = new List<NeedleArticle>
                {
                    new NeedleArticle(),
                    new NeedleArticle()
                }
            };

            // Act
            var parts = ((IArticleParts) dummy).Parts;

            // Assert
            Assert.AreEqual(3, parts.Count, "Invalid number of parts in container!");
            Assert.AreEqual(1, parts.Count(p => p.Name == nameof(dummy.WatchFace)), "Single part saved more than once!");
            Assert.AreEqual(2, parts.Count(p => p.Name == nameof(dummy.Neddles)), "Collection was not saved correctly!");
            // Check type to be sure
            Assert.IsInstanceOf<WatchFaceArticle>(parts.First(p => p.Name == nameof(dummy.WatchFace)).Article, "Type of part does not match!");
            Assert.IsInstanceOf<NeedleArticle>(parts.First(p => p.Name == nameof(dummy.Neddles)).Article, "Type of collection value does not match!");
        }

        [Test]
        public void FromParts()
        {
            // Arrange
            var dummy = new WatchArticle();
            var parts = ((IArticleParts) dummy).Parts;

            // Act
            parts.Add(new ArticlePart(nameof(dummy.WatchFace), new WatchFaceArticle()));
            parts.Add(new ArticlePart(nameof(dummy.Neddles), new NeedleArticle()));
            parts.Add(new ArticlePart(nameof(dummy.Neddles), new NeedleArticle()));

            // Assert
            Assert.NotNull(dummy.WatchFace, "Part not set from container!");
            Assert.NotNull(dummy.Neddles, "Collection is null!");
            Assert.AreEqual(2, dummy.Neddles.Count, "Number of parts invalid!");
        }

        [Test]
        public void WithMissingPart()
        {
            // Arrange
            var dummy = new WatchArticle();
            var parts = ((IArticleParts)dummy).Parts;

            // Act
            parts.Add(new ArticlePart(nameof(dummy.Neddles), new NeedleArticle()));
            parts.Add(new ArticlePart(nameof(dummy.Neddles), new NeedleArticle()));

            // Assert
            Assert.NotNull(dummy.Neddles, "Collection null!");
            Assert.AreEqual(2, dummy.Neddles.Count, "Collection count invalid!");
            Assert.IsNull(dummy.WatchFace, "Part must be null if it was not set!");
        }

        [Test]
        public void WithoutParts()
        {
            // Arrange
            var dummy = new WatchArticle();
            var parts = ((IArticleParts)dummy).Parts;

            // Act
            parts.Add(new ArticlePart(nameof(dummy.WatchFace), new WatchFaceArticle()));

            // Assert
            Assert.NotNull(dummy.WatchFace, "Part not set from container!");
            Assert.NotNull(dummy.Neddles, "Collection is null!");
            Assert.AreEqual(0, dummy.Neddles.Count, "There should be no parts in the collection");
        }
    }
}
