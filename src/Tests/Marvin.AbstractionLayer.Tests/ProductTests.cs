// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Marvin.AbstractionLayer.Products;
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
            var watch = new WatchType()
            {
                Identity = new ProductIdentity("1277125", 01),
                Watchface = new ProductPartLink<WatchfaceType>
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
            Assert.AreEqual(watch, watchInstance.ProductType, "Wrong watch product");
            Assert.AreEqual(watch.Watchface.Product, watchInstance.Watchface.ProductType, "Wrong watchface product");
            Assert.AreEqual(NeedleRole.Hours, watch.Needles.ElementAt(0).Role, "Role not set on instance");
        }
    }
}
