// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Modules;
using NUnit.Framework;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class HandlingWriteAndRead : OpcUaTestBase
{
    [SetUp]
    public async Task Setup()
    {
        await BasicSetup();
        _driver._session = _sessionMock.Object;
        await ((IAsyncPlugin)_driver).StartAsync();
    }

    [Test]
    public void TestSendPrimitiveValue()
    {
        //Arrange
        var node = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable).Value;
        var msg = new OpcUaMessage()
        {
            Identifier = node.NodeId.ToString(),
            Payload = 5
        };

        //Act
        _driver.Send(msg);

        //Assert
        _sessionMock.Verify(s => s.WriteAsync(null, It.IsAny<WriteValueCollection>(), It.IsAny<CancellationToken>()));

    }

    [Test]
    public void TestSetOutput()
    {
        //Arrange
        var node = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable).Value;
        var value = 5;

        //Act
        _driver.Output[node.NodeId.ToString()] = value;

        //Assert
        _sessionMock.Verify(s => s.WriteAsync(null, It.IsAny<WriteValueCollection>(), It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestRead()
    {
        //Arrange
        var (nodeId, node) = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var resultValue = 8;
        SetupRead(resultValue);

        //Act
        var value = _driver.ReadNodeAsync(node.NodeId.ToString());

        //Assert
        _sessionMock.Verify(s => s.ReadValueAsync(It.IsAny<NodeId>(), It.IsAny<CancellationToken>()));
    }

    [Test]
    public void TestGetInput()
    {
        //Arrange
        var (nodeId, node) = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var resultValue = 8;
        SetupRead(resultValue);

        //Act
        var value = _driver.Input[node.NodeId.ToString()];

        //Assert
        _sessionMock.Verify(s => s.ReadValueAsync(It.IsAny<NodeId>(), It.IsAny<CancellationToken>()));
        Assert.That(value, Is.EqualTo(resultValue));
    }

    [Test]
    public void TestReadOuput()
    {
        //Arrange
        var (nodeId, node) = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var resultValue = 8;
        SetupRead(resultValue);

        //Act
        var value = _driver.Output[node.NodeId.ToString()];

        //Assert
        _sessionMock.Verify(s => s.ReadValueAsync(It.IsAny<NodeId>(), It.IsAny<CancellationToken>()));
        Assert.That(value, Is.EqualTo(resultValue));
    }

    private void SetupRead(object resultValue)
    {
        _sessionMock.Setup(s => s.ReadValueAsync(It.IsAny<NodeId>(), It.IsAny<CancellationToken>())).Callback((NodeId nodeId, CancellationToken cancellationToken) =>
        {
            Assert.That(nodeId.Identifier, Is.EqualTo(nodeId.Identifier));
            Assert.That(nodeId.NamespaceIndex, Is.EqualTo(nodeId.NamespaceIndex));
        }).ReturnsAsync(new DataValue() { Value = resultValue, StatusCode = 0 });

    }

    [Test]
    public void TestFindNode()
    {
        //Arrange
        var (_, node) = _rootNodes.FirstOrDefault(n => n.Value.NodeClass == NodeClass.Variable);
        var nodeId = OpcUaNode.CreateExpandedNodeId(node.NodeId.ToString());

        //Act
        var result = _driver.FindNodeId(node.DisplayName.ToString());

        //Assert
        Assert.That(result.Any(x => x.Equals(nodeId)), Is.True);
    }
}
