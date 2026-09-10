// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.AbstractionLayer.Resources;
using Moryx.AbstractionLayer.Tests.TestData;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class ResourceConstructorAttributeTests
{
    [Test]
    public void GetCustomAttribute_OnOverridenResourceConstructorMethod_IsNotInherited()
    {
        var method = typeof(SpecificConstructorResource).GetMethod(nameof(SpecificConstructorResource.ConstructGeneralResource));
        var attribute = method.GetCustomAttributes(typeof(ResourceConstructorAttribute), true);

        Assert.That(attribute, Is.Empty,
            "ResourceConstructorAttribute must not be derived because a resource constructor can be used in base " +
            "classes for general resource constructors which however should be hidden in more specific derived " +
            "classes which in turn could define different resource constructor methods.");
    }
}
