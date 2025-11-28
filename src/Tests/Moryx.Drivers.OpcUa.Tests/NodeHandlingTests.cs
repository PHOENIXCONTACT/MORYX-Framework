// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Drivers.OpcUa.States;
using Moryx.Modules;
using NUnit.Framework;
using Opc.Ua;
using Opc.Ua.Client;

namespace Moryx.Drivers.OpcUa.Tests;

//TODO: Remove all state based tests - States are internals of the driver. The driver should be tested from outside.

[TestFixture]
public class NodeHandlingTests : OpcUaTestBase
{
    private const string Value = "value";
    [SetUp]
    public void Setup()
    {
        BasicSetup();
        _driver._session = _sessionMock.Object;
    }

    [Test(Description = "Number of nodes browsed fits")]
    public void TestBrowsingNodes()
    {
        //Arrange
        uint subscriptionId = 12;
        double revisedPublishingInterval = 12;
        uint revisedLifetimeCounter = 5;
        uint revisedKeepAliveCount = 5;

        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e is RunningState)
            {
                //Assert II
                Assert.That(_driver.Nodes, Has.Count.EqualTo(_rootNodes.Count), "Number of browsed nodes doesn't fit");
                wait.Set();
            }
        };
        //Act
        ((IPlugin)_driver).Start();

        //Assert I
        _sessionMock.Verify(s => s.AddSubscription(It.IsAny<Subscription>()), "Subscription was not added to the session");
        _sessionMock.Verify(s => s.CreateSubscription(null,
            _driver.PublishingInterval,
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<uint>(),
            It.IsAny<bool>(),
            It.IsAny<byte>(),
            out subscriptionId,
            out revisedPublishingInterval,
            out revisedLifetimeCounter,
            out revisedKeepAliveCount), "Subscription was created with the wrong parameters or not created at all");

        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    [Test(Description = "Properties of the nodes are updated after browsing")]
    public void TestUpdatingNodeAfterBrowsing()
    {
        //Arrange
        var expectedNode = _rootNodes.First();
        var channel = _driver.Channel(OpcUaNode.CreateExpandedNodeId(expectedNode.Value.NodeId.ToString()));
        var node = channel as OpcUaNode;
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e is RunningState)
            {
                //Assert II
                Assert.That(node?.DisplayName, Is.EqualTo(expectedNode.Value.DisplayName.Text));
                Assert.That(node?.BrowseName.ToString(), Is.EqualTo(expectedNode.Value.BrowseName.ToString()));
                wait.Set();
            }
        };
        //Act
        ((IPlugin)_driver).Start();

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
    public void TestSubscribingMonitoredItemsAfterBrowsing()
    {
        //Arrange
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable);
        var channel = _driver.Channel(expectedNode.Value.NodeId.ToString());
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e is RunningState)
            {
                wait.Set();
            }
        };

        //Act          
        channel.Received += DoSomething;
        ((IPlugin)_driver).Start();

        //Assert I
        MonitoredItemCreateResultCollection results2;
        DiagnosticInfoCollection diagnosticInfos2;
        _sessionMock.Verify(s => s.CreateMonitoredItems(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(), It.IsAny<MonitoredItemCreateRequestCollection>()
            , out results2, out diagnosticInfos2));
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    private void DoSomething(object? sender, object e)
    {
        //
    }

    [Test(Description = "Nodes can only be subscribed ones")]
    public void TestNodesCanOnlyBeSubscribedOnes()
    {
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable);
        var channel = _driver.Channel(expectedNode.Value.NodeId.ToString());
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e is RunningState)
            {
                wait.Set();
            }
        };

        //Act          
        channel.Received += DoSomething;
        ((IPlugin)_driver).Start();
        _driver.SubscribeNode(channel.Identifier);

        //Assert I
        MonitoredItemCreateResultCollection results2;
        DiagnosticInfoCollection diagnosticInfos2;
        _sessionMock.Verify(s => s.CreateMonitoredItems(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(), It.IsAny<MonitoredItemCreateRequestCollection>()
            , out results2, out diagnosticInfos2), Times.Once, "Subscription was done never or several times instead of once");
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
    }

    [Test(Description = "Test received events from the driver and the channel, when subscription changed")]
    public void TestSubcribedValueChanges()
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
            if (e is RunningState)
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

        ((IPlugin)_driver).Start();
        _driver.SubscribeNode(node.Identifier);

        // Act
        _driver.OnSubscriptionChanged(node.NodeId, Value);

        //Assert I
        Assert.That(wait.WaitOne(TimeSpan.FromSeconds(2)), "Driver was not running");
        Assert.That(waitSubscription1.WaitOne(TimeSpan.FromSeconds(2)), "Channel doesn't raise received Event");
        Assert.That(waitSubscription2.WaitOne(TimeSpan.FromSeconds(2)), "Driver doesn't raise received Event");
    }

    private void CheckReceivedValue(object receivedValue, object expectedValue)
    {
        //Asert II
        Assert.That(receivedValue.GetType(), Is.EqualTo(expectedValue.GetType()), "Received object has the wrong type");
        Assert.That(receivedValue, Is.EqualTo(expectedValue), "Received object has the wrong value");
    }

    [Test(Description = "Use Aliases for node Ids")]
    public void TestUseNodeIdAliases()
    {
        //Arrange
        const string alias = "whatever";
        var nodeId = NodeId.ToExpandedNodeId(_rootNodes.First().Key, _namespaceTable);
        _driver._nodeIdAliasDictionary.Add(alias, nodeId.ToString());
        ((IPlugin)_driver).Start();

        //Act
        var channel = _driver.Channel(alias);

        //Assert
        Assert.That(OpcUaNode.CreateExpandedNodeId(nodeId.ToString()), Is.EqualTo(channel.Identifier));
    }
}
