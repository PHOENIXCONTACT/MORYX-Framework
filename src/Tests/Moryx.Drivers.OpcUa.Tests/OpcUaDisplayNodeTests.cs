// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class OpcUaDisplayNodeTests
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private ExpandedNodeId _nodeId;
    private OpcUaDisplayNode _displayNode;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

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
