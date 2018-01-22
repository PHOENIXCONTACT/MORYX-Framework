using System.Linq;
using Marvin.Products.Samples;
using NUnit.Framework;

namespace Marvin.AbstractionLayer.Tests
{
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public void InstantiateProduct()
        {
            var watch = new WatchProduct()
            {
                Identity = new ProductIdentity("1277125", 01)
            };

            watch.Watchface.Product = new WatchfaceProduct
            {
                Identity = new ProductIdentity("512380125", 01)
            };

            for (int i = 1; i <= 4; i++)
            {
                watch.Needles.Add(new NeedlePartLink
                {
                    Role = (NeedleRole)i-1,
                    Product = new NeedleProduct
                    {
                        Identity = new ProductIdentity("12641" + i, (short)(i % 4))
                    }
                });
            }


            // Create article instance
            var watchInstance = (WatchArticle)watch.CreateInstance();

            // Assert
            Assert.AreEqual(watch, watchInstance.Product, "Wrong watch product");
            Assert.AreEqual(watch.Watchface.Product, watchInstance.Watchface.Product, "Wrong watchface product");
            Assert.AreEqual(NeedleRole.Hours, watch.Needles.ElementAt(0).Role, "Role not set on instance");
        }
    }
}