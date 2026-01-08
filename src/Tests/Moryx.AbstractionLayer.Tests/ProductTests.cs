// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.Products.Samples;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class ProductTests
{
    [Test]
    public void InstantiateProduct()
    {
        var watch = new WatchType()
        {
            Identity = new ProductIdentity("1277125", 01),
            WatchFace = new ProductPartLink<WatchFaceTypeBase>
            {
                Product = new WatchFaceType
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
        Assert.That(watchInstance.Type, Is.EqualTo(watch), "Wrong watch product");
        Assert.That(watchInstance.WatchFace.Type, Is.EqualTo(watch.WatchFace.Product), "Wrong watchface product");
        Assert.That(watch.Needles.ElementAt(0).Role, Is.EqualTo(NeedleRole.Hours), "Role not set on instance");
    }
}