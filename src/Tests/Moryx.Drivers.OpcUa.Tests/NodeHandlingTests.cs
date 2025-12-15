// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Drivers;
using Moryx.Drivers.OpcUa.States;
using Moryx.Modules;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class NodeHandlingTests : OpcUaTestBase
{
    private const string Value = "value";

    [SetUp]
    public async Task Setup()
    {
        await BasicSetup();
        _driver._session = _sessionMock.Object;
    }

    [Test(Description = "Number of nodes browsed fits")]
    public async Task TestBrowsingNodes()
    {
        //Arrange
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                //Assert II
                Assert.That(_driver.Nodes, Has.Count.EqualTo(_rootNodes.Count), "Number of browsed nodes doesn't fit");
                wait.Set();
            }
        };

        //Act
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert I
        _sessionMock.Verify(s => s.AddSubscription(It.IsAny<Subscription>()), "Subscription was not added to the session");
        _sessionMock.Verify(s => s.CreateSubscriptionAsync(null,
            _driver.PublishingInterval,
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<byte>(),
            It.IsAny<CancellationToken>()
            ), "Subscription was created with the wrong parameters or not created at all");

        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    [Test(Description = "Properties of the nodes are updated after browsing")]
    public async Task TestUpdatingNodeAfterBrowsing()
    {
        //Arrange
        var expectedNode = _rootNodes.First();
        var channel = _driver.Channel(OpcUaNode.CreateExpandedNodeId(expectedNode.Value.NodeId.ToString()));
        var node = channel as OpcUaNode;
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                //Assert II
                Assert.That(node?.DisplayName, Is.EqualTo(expectedNode.Value.DisplayName.Text));
                Assert.That(node?.BrowseName.ToString(), Is.EqualTo(expectedNode.Value.BrowseName.ToString()));
                wait.Set();
            }
        };
        //Act
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert I
        Assert.That(node, Is.Not.Null);
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    [Test(Description = "Channels without namespace uri won't be created before the driver is running")]
    public void TestReturnChannelBeforeDriverIsRunning()
    {
        //Arrange
        var expectedNode = _rootNodes.First();

        //Act
        var channel = _driver.Channel(expectedNode.Key.ToString());

        //Assert
        Assert.That(channel, Is.Null);
    }

    [Test(Description = "Subscribe monitored items after browsing")]
    public async Task TestSubscribingMonitoredItemsAfterBrowsing()
    {
        //Arrange
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var channel = _driver.Channel(expectedNode.Value.NodeId.ToString());
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                wait.Set();
            }
        };

        //Act
        channel.Received += DoSomething;
        await ((IAsyncPlugin)_driver).StartAsync();

        //Assert I
        MonitoredItemCreateResultCollection results2;
        DiagnosticInfoCollection diagnosticInfos2;
        _sessionMock.Verify(s => s.CreateMonitoredItemsAsync(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(), It.IsAny<MonitoredItemCreateRequestCollection>()
            , It.IsAny<CancellationToken>()));
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    private void DoSomething(object? sender, object e)
    {
        //
    }

    [Test(Description = "Nodes can only be subscribed ones")]
    public async Task TestNodesCanOnlyBeSubscribedOnes()
    {
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var channel = _driver.Channel(expectedNode.Value.NodeId.ToString());
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                wait.Set();
            }
        };

        //Act
        channel.Received += DoSomething;
        await ((IAsyncPlugin)_driver).StartAsync();
        _driver.SubscribeNode(channel.Identifier);

        //Assert I
        MonitoredItemCreateResultCollection results2;
        DiagnosticInfoCollection diagnosticInfos2;
        _sessionMock.Verify(s => s.CreateMonitoredItemsAsync(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(), It.IsAny<MonitoredItemCreateRequestCollection>()
            , It.IsAny<CancellationToken>()), Times.Once, "Subscription was done never or several times instead of once");
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    [Test(Description = "Test received events from the driver and the channel, when subscription changed")]
    public async Task TestSubcribedValueChanges()
    {
        // Arrange
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var node = (OpcUaNode)_driver.Channel(expectedNode.Value.NodeId.ToString());
        var wait = new AutoResetEvent(false);
        var waitSubscription1 = new AutoResetEvent(false);
        var waitSubscription2 = new AutoResetEvent(false);
        var waitSubscription3 = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                wait.Set();
            }
        };

        node.Received += (sender, e) =>
        {
            //Assert II
            waitSubscription1.Set();
            CheckReceivedValue(e, Value);
        };
        (_driver).Received += (sender, e) =>
        {
            waitSubscription2.Set();
            //Assert III
            var msg = e as OpcUaMessage;
            Assert.That(msg, Is.Not.Null, "Message received from the Driver.Received event has the wrong type");
            CheckReceivedValue(msg!.Payload, Value);
        };
        _driver.Input.InputChanged += (sender, e) =>
        {
            waitSubscription3.Set();
            //Assert IV
            Assert.That(e.Key, Is.EqualTo(node.NodeId.ToString()));
        };

        await ((IAsyncPlugin)_driver).StartAsync();
        _driver.SubscribeNode(node.Identifier);

        // Act
        _driver.OnSubscriptionChanged(node.NodeId, Value);

        //Assert I
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
        Assert.That(waitSubscription1.WaitOne(TimeSpan.FromSeconds(2)), "Channel doesn't raise received Event");
        Assert.That(waitSubscription2.WaitOne(TimeSpan.FromSeconds(2)), "Driver doesn't raise received Event");
    }

    private static void CheckReceivedValue(object receivedValue, object expectedValue)
    {
        //Asert II
        Assert.That(receivedValue.GetType(), Is.EqualTo(expectedValue.GetType()), "Received object has the wrong type");
        Assert.That(receivedValue, Is.EqualTo(expectedValue), "Received object has the wrong value");
    }

    [Test(Description = "Use Aliases for node Ids")]
    public async Task TestUseNodeIdAliases()
    {
        //Arrange
        const string alias = "whatever";
        var nodeId = NodeId.ToExpandedNodeId(_rootNodes.First().Key, _namespaceTable);
        _driver._nodeIdAliasDictionary.Add(alias, nodeId.ToString());
        await ((IAsyncPlugin)_driver).StartAsync();

        //Act
        var channel = _driver.Channel(alias);

        //Assert
        Assert.That(OpcUaNode.CreateExpandedNodeId(nodeId.ToString()), Is.EqualTo(channel.Identifier));
    }
}
