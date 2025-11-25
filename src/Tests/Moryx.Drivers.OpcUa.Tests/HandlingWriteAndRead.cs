// Copyright (c) 2025, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moq;
using Moryx.Drivers.OpcUa.Nodes;
using Moryx.Modules;
using NUnit.Framework;
using Opc.Ua;

namespace Moryx.Drivers.OpcUa.Tests;

[TestFixture]
public class HandlingWriteAndRead : OpcUaTestBase
{
    [SetUp]
    public void Setup()
    {
        BasicSetup();
        _driver._session = _sessionMock.Object;
        ((IPlugin)_driver).Start();
    }

    [Test]
    public void TestSendPrimitiveValue()
    {
        //Arrange
        var node = Nodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable).Value;
        var msg = new OpcUaMessage()
        {
            Identifier = node.NodeId.ToString(),
            Payload = 5
        };

        //Act
        _driver.Send(msg);

        //Assert
        StatusCodeCollection results2;
        DiagnosticInfoCollection diagnosticInfos2;
        _sessionMock.Verify(s => s.Write(null, It.IsAny<WriteValueCollection>(), out results2, out diagnosticInfos2));

    }

    [Test]
    public void TestSetOutput()
    {
        //Arrange
        var node = Nodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable).Value;
        var value = 5;

        //Act
        _driver.Output[node.NodeId.ToString()] = value;

        //Assert
        StatusCodeCollection results2;
        DiagnosticInfoCollection diagnosticInfos2;
        _sessionMock.Verify(s => s.Write(null, It.IsAny<WriteValueCollection>(), out results2, out diagnosticInfos2));
    }

    [Test]
    public void TestRead()
    {
        //Arrange
        var (nodeId, node) = Nodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable);
        var resultValue = 8;
        SetupRead(resultValue, nodeId);

        //Act
        var value = _driver.ReadNode(node.NodeId.ToString());

        //Assert
        _sessionMock.Verify(s => s.ReadValue(It.IsAny<NodeId>()));
    }

    [Test]
    public void TestGetInput()
    {
        //Arrange
        var (nodeId, node) = Nodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable);
        var resultValue = 8;
        SetupRead(resultValue, nodeId);

        //Act
        var value = _driver.Input[node.NodeId.ToString()];

        //Assert
        _sessionMock.Verify(s => s.ReadValue(It.IsAny<NodeId>()));
        Assert.That(value, Is.EqualTo(resultValue));
    }

    [Test]
    public void TestReadOuput()
    {
        //Arrange
        var (nodeId, node) = Nodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable);
        var resultValue = 8;
        SetupRead(resultValue, nodeId);

        //Act
        var value = _driver.Output[node.NodeId.ToString()];

        //Assert
        _sessionMock.Verify(s => s.ReadValue(It.IsAny<NodeId>()));
        Assert.That(value, Is.EqualTo(resultValue));
    }

    private void SetupRead(object resultValue, NodeId nodeId)
    {
        _sessionMock.Setup(s => s.ReadValue(It.IsAny<NodeId>())).Callback((NodeId nodeId) =>
        {
            Assert.That(nodeId.Identifier, Is.EqualTo(nodeId.Identifier));
            Assert.That(nodeId.NamespaceIndex, Is.EqualTo(nodeId.NamespaceIndex));
        }).Returns(new DataValue() { Value = resultValue, StatusCode = 0 });

    }

    [Test]
    public void TestFindNode()
    {
        //Arrange
        var (_, node) = Nodes.FirstOrDefault(n => n.Value.NodeClass == Opc.Ua.NodeClass.Variable);
        var nodeId = OpcUaNode.CreateExpandedNodeId(node.NodeId.ToString());

        //Act
        var result = _driver.FindNodeId(node.DisplayName.ToString());

        //Assert           
        Assert.That(result.Any(x => x.Equals(nodeId)), Is.True);
    }
}
