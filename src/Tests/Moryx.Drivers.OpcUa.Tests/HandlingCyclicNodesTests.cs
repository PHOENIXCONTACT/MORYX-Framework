// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using System.Collections;
using System.Text.Json;
using Moq;
using Moryx.AbstractionLayer.Drivers;
using Moryx.Modules;
using Moryx.Tools;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class HandlingCyclicNodesTests : OpcUaTestBase
{
    private new readonly NamespaceTable _namespaceTable = CreateNamespaceTable();

    [SetUp]
    public async Task Setup()
    {
        ReflectionTool.TestMode = true;

        _sessionMock = new Mock<ISession>();
        _sessionMock.Setup(s => s.NamespaceUris).Returns(_namespaceTable);
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Returns(true);

        _sessionMock.Setup(s => s.CreateSubscriptionAsync(
            null,
            It.IsAny<double>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<byte>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CreateSubscriptionResponse()
            {
                SubscriptionId = 12,
                RevisedPublishingInterval = 12,
                RevisedLifetimeCount = 5,
                RevisedMaxKeepAliveCount = 5
            });

        var result = new MonitoredItemCreateResult(0);
        MonitoredItemCreateResultCollection results = [result];
        _sessionMock
            .Setup(s => s.CreateMonitoredItemsAsync(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(),
                It.IsAny<MonitoredItemCreateRequestCollection>(), It.IsAny<CancellationToken>()));
        _sessionMock
            .Setup(s => s.SetPublishingModeAsync(null, It.IsAny<bool>(), It.IsAny<UInt32Collection>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SetPublishingModeResponse()
            {
                Results = [StatusCodes.Good],
                DiagnosticInfos = []
            });

        var subscriptionFactoryMock = CreateSubscriptionFactoryMock(_sessionMock.Object);

        _driver = await CreateDriver(subscriptionFactoryMock.Object);
    }

    private void SetupDriver()
    {
        _sessionMock.Setup(s => s.AddSubscription(It.IsAny<Subscription>())).Callback((Subscription sub) =>
        {
            var prop = sub.GetType().GetProperties().FirstOrDefault(propInfo => propInfo.Name.Equals(nameof(sub.Session)));
            prop?.SetValue(sub, _sessionMock.Object);
        });
        _driver._session = _sessionMock.Object;
    }

    [Test(Description = "Do not show cyclic nodes in the UI")]
    public async Task DoNotShowDirectCyclicNodesInTheUi()
    {
        //Arrange
        SetupDirectCyclicNodes();
        SetupDriver();

        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (_, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                //Assert II
                Assert.That(_driver.Nodes.Count, Is.EqualTo(1), "Number of browsed nodes doesn't fit");
                Assert.DoesNotThrow(() => JsonSerializer.Serialize(_driver.Nodes));
                wait.Set();
            }
        };

        //Act
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), Is.True, "Driver was not running");
    }

    /// <summary>
    /// Sets up a cyclic node hierarchy:
    ///     ROOT
    ///     ├─ NODE_1
    ///        ├─ NODE_1
    ///           ├─ NODE_1
    ///           ...
    /// </summary>
    private void SetupDirectCyclicNodes()
    {
        var nextRefs = CreateNodes(_namespaceTable);
        ByteStringCollection? byteArray = null;
        var nextRefsDefault = new ReferenceDescriptionCollection() { nextRefs[0] };

        _sessionMock.Setup(s => s.BrowseAsync(null, null, new List<NodeId> { ObjectIds.RootFolder }, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
    true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [nextRefsDefault], null));

        _sessionMock.Setup(s => s.BrowseAsync(null, null, It.Is<List<NodeId>>(x => x.First() != ObjectIds.RootFolder &&
            x.First().ToString() != ExpandedNodeId.ToNodeId(nextRefs[0].NodeId, _namespaceTable).ToString()), It.IsAny<uint>(), It.IsAny<BrowseDirection>(),
            ReferenceTypeIds.HierarchicalReferences, true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [nextRefsDefault], null))
            ;
    }

    [Test(Description = "Do not show cyclic nodes in the UI")]
    public async Task DoNotShowIndirectCyclicNodesInTheUi()
    {
        //Arrange
        SetupIndirectCyclicNodes();
        SetupDriver();

        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (_, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                //Assert II
                var node = (OpcUaObjectDisplayNode)_driver.Nodes[0];
                Assert.That(_driver.Nodes.Count, Is.EqualTo(1), "Number of browsed nodes doesn't fit");
                Assert.That(node.BrowseName, Is.EqualTo("browsename1"));

                Assert.That(node.Nodes.Count, Is.EqualTo(1), "Number of subnodes doesn't fit");
                Assert.That(node.Nodes[0].BrowseName, Is.EqualTo("browsename2"));
                Assert.DoesNotThrow(() => JsonSerializer.Serialize(_driver.Nodes));
                wait.Set();
            }
        };

        //Act
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), Is.True, "Driver was not running");
    }

    /// <summary>
    /// Sets up a cyclic node hierarchy like this:
    ///     ROOT
    ///      ├─ NODE_1
    ///         ├─ NODE_2
    ///            ├─ NODE_1
    ///               ├─ NODE_2
    ///               ...
    /// </summary>
    private void SetupIndirectCyclicNodes()
    {
        var nextRefs = CreateNodes(_namespaceTable);
        var node1 = nextRefs[0];
        var node2 = new ReferenceDescription()
        {
            NodeId = new ExpandedNodeId("identifier2", 1, _namespaceTable.GetString(1), 0),
            DisplayName = "abcd",
            NodeClass = NodeClass.Object,
            BrowseName = "browsename2"
        };
        ByteStringCollection? byteArray = null;

        var rootResult = new ReferenceDescriptionCollection() { node1 };
        _sessionMock.Setup(s => s.BrowseAsync(null, null, new List<NodeId> { ObjectIds.RootFolder }, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [rootResult], null));

        var node1Result = new ReferenceDescriptionCollection() { node2 };
        _sessionMock.Setup(s => s.BrowseAsync(null, null, It.Is<List<NodeId>>(x => x.First() != ObjectIds.RootFolder &&
            x.First().ToString() == ExpandedNodeId.ToNodeId(node1.NodeId, _namespaceTable).ToString()), It.IsAny<uint>(), It.IsAny<BrowseDirection>(),
            ReferenceTypeIds.HierarchicalReferences, true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [node1Result], null));

        var node2Result = new ReferenceDescriptionCollection() { node1 };
        _sessionMock.Setup(s => s.BrowseAsync(null, null, It.Is<List<NodeId>>(x => x.First() != ObjectIds.RootFolder &&
            x.First().ToString() == ExpandedNodeId.ToNodeId(node2.NodeId, _namespaceTable).ToString()), It.IsAny<uint>(), It.IsAny<BrowseDirection>(),
            ReferenceTypeIds.HierarchicalReferences, true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [node2Result], null));
    }

    [Test(Description = "If a node exists on different branches (is not cyclic), then it should be resolved")]
    public async Task ShowSameNodeOnDifferentBranch()
    {
        //Arrange
        SetupReoccuringNodes();
        SetupDriver();

        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (_, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                //Assert II
                Assert.That(_driver.Nodes.Count, Is.EqualTo(3), "Number of browsed nodes doesn't fit");
                Assert.That(((OpcUaObjectDisplayNode)_driver.Nodes[0]).Nodes.Count, Is.EqualTo(1), "First node should have a subnode");
                Assert.That(((OpcUaObjectDisplayNode)_driver.Nodes[0]).Nodes[0].BrowseName, Is.EqualTo("browsename2"), "First node should have `identifier2` as subnode");
                Assert.DoesNotThrow(() => JsonSerializer.Serialize(_driver.Nodes));
                wait.Set();
            }
        };

        //Act
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), Is.True, "Driver was not running");
    }

    /// <summary>
    /// Sets up a this hierarchy:
    ///     ROOT
    ///     ├─ NODE_1
    ///     |  ├─ NODE_2
    ///     ├─ NODE_2
    ///     ├─ NODE_3
    /// </summary>
    private void SetupReoccuringNodes()
    {
        var nextRefs = CreateNodes(_namespaceTable);
        var node1 = nextRefs[0];
        var node2 = nextRefs[1];
        var node3 = nextRefs[2];
        ByteStringCollection? byteArray = null;

        var rootResult = nextRefs;
        _sessionMock.Setup(s => s.BrowseAsync(null, null, new List<NodeId> { ObjectIds.RootFolder }, It.IsAny<uint>(), It.IsAny<BrowseDirection>(), ReferenceTypeIds.HierarchicalReferences,
            true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [rootResult], null));

        var node1Result = new ReferenceDescriptionCollection() { node2 };
        _sessionMock.Setup(s => s.BrowseAsync(null, null, It.Is<List<NodeId>>(x => x.First() != ObjectIds.RootFolder &&
            x.First().ToString() == ExpandedNodeId.ToNodeId(node1.NodeId, _namespaceTable).ToString()), It.IsAny<uint>(), It.IsAny<BrowseDirection>(),
            ReferenceTypeIds.HierarchicalReferences, true, It.IsAny<uint>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((null, byteArray, [node1Result], null));
    }
}
