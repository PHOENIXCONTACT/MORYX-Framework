// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class OpcUaDisplayNodeTests
{
    private ExpandedNodeId _nodeId;
    private OpcUaDisplayNode _displayNode;

    [SetUp]
    public void SetUp()
    {
        _nodeId = new ExpandedNodeId(1234, "http://namespace.org/", 0);
        _displayNode = new OpcUaDisplayNode(_nodeId)
        {
            DisplayName = "NodeDisplayName"
        };
    }

    [Test]
    public void NodeIdContainsNamespaceUri()
    {
        Assert.That(_displayNode.NodeId, Is.EqualTo("nsu=http://namespace.org/;i=1234"));
    }

    [Test]
    public void DisplayName()
    {
        Assert.That(_displayNode.DisplayName, Is.EqualTo("NodeDisplayName"));
    }

    [Test]
    public void ToStringCombinesIdAndDisplayName()
    {
        var displayNodeAsString = _displayNode.ToString();

        Assert.That(displayNodeAsString, Is.EqualTo("i=1234 - NodeDisplayName"));
    }

}
