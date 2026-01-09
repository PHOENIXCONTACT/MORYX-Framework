// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using NUnit.Framework;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class NodeIdStringTests : OpcUaTestBase
{
    private new readonly NamespaceTable _namespaceTable = new(["http://opcfoundation.org/UA/", "http://anothernamespace.org/"]);

    private const string NodeIdWithUriOnly = "nsu=http://anothernamespace.org/;i=2994";
    private const string NodeIdWithNamespaceOnly = "ns=1;i=2994";
    private const string NodeIdWithDefaultNamespaceOnly = "ns=0;i=2994";
    private const string NodeIdWithUriAndIndex = "nsu=http://anothernamespace.org/;ns=1;i=2994";

    [Test]
    public void NamespaceIndexIsIgnoredWhenUriIsSpecified()
    {
        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithUriAndIndex);
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://anothernamespace.org/;i=2994"));
    }

    [Test]
    public void NamespaceUriIsUsedWhenIndexIsNotSpecified()
    {
        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithUriOnly);
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://anothernamespace.org/;i=2994"));
    }

    [Test]
    public void NamespaceIndexIsUsedWhenUriIsNotSpecified()
    {
        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithNamespaceOnly);
        Assert.That(nodeIdString, Is.EqualTo("ns=1;i=2994"));
    }

    [Test]
    public void NamespaceIndexIgnoredWhenZeroAndUriIsNotSpecified()
    {
        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithDefaultNamespaceOnly);
        Assert.That(nodeIdString, Is.EqualTo("i=2994"));
    }

    [Test]
    public void NamespaceIndexIsIgnoredWhenUriIsSpecifiedAtNodeIdWithDefaultNamespace()
    {
        var nodeId = new ExpandedNodeId("2994", 0, _namespaceTable.GetString(0), 0);

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(nodeId.ToString());
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://opcfoundation.org/UA/;s=2994"));
    }

    [Test]
    public void NamespaceIndexIsIgnoredWhenUriIsSpecifiedAtNodeId()
    {
        var nodeId = new ExpandedNodeId("2994", 1, _namespaceTable.GetString(1), 0);

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(nodeId.ToString());
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://anothernamespace.org/;s=2994"));
    }

    [Test]
    public void NamespaceIndexIsUsedWhenUriIsNotSpecifiedAtNodeId()
    {
        var nodeId = new ExpandedNodeId(2994, 1);
        var nodeIdString = OpcUaNode.CreateExpandedNodeId(nodeId.ToString());

        Assert.That(nodeIdString, Is.EqualTo("ns=1;i=2994"));
    }

    [Test]
    public void NamespaceIndexIgnoredWhenZeroAndUriIsNotSpecifiedAtNodeId()
    {
        var nodeId = new ExpandedNodeId(2994, 0);
        var nodeIdString = OpcUaNode.CreateExpandedNodeId(nodeId.ToString());

        Assert.That(nodeIdString, Is.EqualTo("i=2994"));
    }

    [Test]
    public void NamespaceUriIsUsedWhenIndexIsNotSpecifiedAtNodeId()
    {
        var nodeId = new ExpandedNodeId(2994, _namespaceTable.GetString(1));

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(nodeId.ToString());
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://anothernamespace.org/;i=2994"));
    }
}
