// Copyright (c) 2026 Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.AbstractionLayer.Drivers;
using Moryx.Modules;
using NUnit.Framework;
using Opc.Ua;

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

    [Test(Description = "Channels without namespace uri won't be created before the driver is running")]
    public async Task TestReturnChannelBeforeDriverIsRunning()
    {
        //Arrange
        var expectedNode = _rootNodes.First();

        //Act
        var channel = _driver.Channel(expectedNode.Key.ToString());

        //Assert
        Assert.That(channel, Is.Null);
    }

    [Test(Description = "Nodes can only be subscribed once")]
    public async Task TestNodesCanOnlyBeSubscribedOnce()
    {
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var nodeId = expectedNode.Value.NodeId.ToString();
        var wait = new AutoResetEvent(false);
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                wait.Set();
            }
        };

        //Act
        await ((IAsyncPlugin)_driver).StartAsync();
        var wasRunning = wait.WaitOne(TimeSpan.FromSeconds(2));
        _driver.SubscribeNode(nodeId);
        _driver.SubscribeNode(nodeId);

        //Assert I
        _sessionMock.Verify(s => s.CreateMonitoredItemsAsync(null, It.IsAny<uint>(), It.IsAny<TimestampsToReturn>(), It.IsAny<MonitoredItemCreateRequestCollection>()
            , It.IsAny<CancellationToken>()), Times.Once, "Subscription was done never or several times instead of once");
        Assert.That(wasRunning, "Driver was not running");
    }

    [Test(Description = "Nodes can only be subscribed once")]
    [Ignore("This conflicts with `TestReturnChannelBeforeDriverIsRunning`. Decision on expected behaviour to be made.")]
    public async Task CannotReadNodesInDisconnectedState()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            // Act
            var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
            var channel = _driver.Channel(expectedNode.Value.NodeId.ToString());
        });
    }

    [Test(Description = "Test received events from the driver and the channel, when subscription changed")]
    public async Task TestSubcribedValueChanges()
    {
        // Arrange
        var expectedNode = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var wait = new AutoResetEvent(false);
        await ((IAsyncPlugin)_driver).StartAsync();
        var wasRunning = wait.WaitOne(TimeSpan.FromSeconds(2));
        var node = (OpcUaNode)_driver.Channel(expectedNode.Value.NodeId.ToString());

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

        _driver.SubscribeNode(node.Identifier);

        // Act
        _driver.OnSubscriptionChanged(node.NodeId, Value);

        //Assert I
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
        var wait = new AutoResetEvent(false);
        const string alias = "nodeAlias";
        var nodeId = NodeId.ToExpandedNodeId(_rootNodes.First().Key, NamespaceTable);
        _driver._nodeIdAliasDictionary.Add(alias, nodeId.ToString());
        _driver.StateChanged += (sender, e) =>
        {
            if (e.Classification == StateClassification.Running)
            {
                wait.Set();
            }
        };
        await ((IAsyncPlugin)_driver).StartAsync();
        wait.WaitOne(TimeSpan.FromSeconds(2));

        //Act
        var channel = _driver.Channel(alias);

        //Assert
        Assert.That(OpcUaNode.CreateExpandedNodeId("ns=1;s=identifier1"), Is.EqualTo(channel.Identifier));
    }
}
