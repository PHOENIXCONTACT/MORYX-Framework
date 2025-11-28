// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG

using Moq;
using Moryx.AbstractionLayer.Drivers.Message;
using Moryx.Drivers.OpcUa.DriverStates;
using Moryx.Modules;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class NodeIdStringTests : OpcUaTestBase
{
    private NamespaceTable _namespaceTable = new(["http://opcfoundation.org/UA/", "http://anothernamespace.org/"]);

    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void NamespaceIndexIsIgnoredWhenUriIsSpecified()
    {
        var NodeIdWithUriAndIndex = "nsu=http://anothernamespace.org/;ns=1;i=2994";

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithUriAndIndex);
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://anothernamespace.org/;i=2994"));
    }

    [Test]
    public void NamespaceUriIsUsedWhenIndexIsNotSpecified()
    {
        var NodeIdWithUriOnly = "nsu=http://anothernamespace.org/;i=2994";

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithUriOnly);
        Assert.That(nodeIdString, Is.EqualTo("nsu=http://anothernamespace.org/;i=2994"));
    }

    [Test]
    public void NamespaceIndexIsUsedWhenUriIsNotSpecified()
    {
        var NodeIdWithUriOnly = "ns=1;i=2994";

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithUriOnly);
        Assert.That(nodeIdString, Is.EqualTo("ns=1;i=2994"));
    }

    [Test]
    public void NamespaceIndexIgnoredWhenZeroAndUriIsNotSpecified()
    {
        var NodeIdWithUriOnly = "ns=0;i=2994";

        var nodeIdString = OpcUaNode.CreateExpandedNodeId(NodeIdWithUriOnly);
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
