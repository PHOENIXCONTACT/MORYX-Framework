// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Linq;
using Moryx.AbstractionLayer.Products;
using Moryx.AbstractionLayer.Resources;
using Moryx.Resources.Samples;
using NUnit.Framework;

namespace Moryx.AbstractionLayer.Tests;

[TestFixture]
public class ResourceExtensionsGetFirstRelatedResourceTests
{
    [Test]
    public void WhenResourceAvailable_ReturnsFirstRelatedResource()
    {
        var machine = new Machine();
        var mountingCell = new MountingCell();
        var solderingCell = new SolderingCell();
        machine.Parent = mountingCell;
        mountingCell.Parent = solderingCell;

        var firstRelatedResource = machine.GetFirstRelatedResource(r => r is SolderingCell, r => r.Parent);

        Assert.That(firstRelatedResource, Is.SameAs(solderingCell));
    }

    [Test]
    public void WhenNoMatchingResource_ReturnsNull()
    {
        var machine = new Machine();
        var mountingCell = new MountingCell();
        machine.Parent = mountingCell;
        mountingCell.Parent = null;

        var firstRelatedResource = machine.GetFirstRelatedResource(r => r is SolderingCell, r => r.Parent);

        Assert.That(firstRelatedResource, Is.Null);
    }

    [Test]
    public void UsingListExtension_WhenResourceInTree_ReturnsResource()
    {
        var machine = new Machine();
        var mountingCell = new MountingCell();
        var solderingCell = new SolderingCell();
        machine.Parent = mountingCell;
        mountingCell.Parent = solderingCell;

        var firstRelatedResource = machine.GetFirstRelatedResource(r => r is SolderingCell, r => [r.Parent]);

        Assert.That(firstRelatedResource, Is.SameAs(solderingCell));
    }

    [Test]
    public void UsingListExtension_WhenNoMatchingResource_ReturnsNull()
    {
        var machine = new Machine();
        var mountingCell = new MountingCell();
        machine.Parent = mountingCell;
        mountingCell.Parent = null;

        var firstRelatedResource = machine.GetFirstRelatedResource(r => r is SolderingCell, r => [r.Parent]);

        Assert.That(firstRelatedResource, Is.Null);
    }
}
