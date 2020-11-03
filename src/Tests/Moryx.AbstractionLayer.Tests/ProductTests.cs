// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Samples;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests
{
    [TestFixture]
    public class ProductTests
    {
        [Test]
        public void InstantiateProduct()
        {
            var watch = new WatchType()
            {
                Identity = new ProductIdentity("1277125", 01),
                Watchface = new ProductPartLink<WatchfaceTypeBase>
                {
                    Product = new WatchfaceType
                    {
                        Identity = new ProductIdentity("512380125", 01)
                    }
                }
            };

            for (int i = 1; i <= 4; i++)
            {
                watch.Needles.Add(new NeedlePartLink
                {
                    Role = (NeedleRole)i - 1,
                    Product = new NeedleType
                    {
                        Identity = new ProductIdentity("12641" + i, (short)(i % 4))
                    }
                });
            }


            // Create instance
            var watchInstance = (WatchInstance)watch.CreateInstance();

            // Assert
            Assert.AreEqual(watch, watchInstance.Type, "Wrong watch product");
            Assert.AreEqual(watch.Watchface.Product, watchInstance.Watchface.Type, "Wrong watchface product");
            Assert.AreEqual(NeedleRole.Hours, watch.Needles.ElementAt(0).Role, "Role not set on instance");
        }
    }
}
